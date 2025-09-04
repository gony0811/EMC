
using System.Data.SQLite;
using System.IO;
using System.Text;

namespace EGGPLANT
{
    public static class DB
    {
        public static readonly string DbDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MyApp");
        public static readonly string DbPath = Path.Combine(DbDir, "app.db");
        public static readonly string SchemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "14 DB", "schema.sql");   // 스키마 파일 경로

        /// <summary>
        /// DB가 없으면 파일 생성 + schema.sql 실행, 있으면 연결만 수행.
        /// </summary>
        public static void EnsureCreatedOnce()
        {
            Directory.CreateDirectory(DbDir);
            bool isNew = !File.Exists(DbPath);

            // (신규일 때만) 물리 파일 생성
            if (isNew)
                SQLiteConnection.CreateFile(DbPath);

            // 연결 문자열: 외래키 ON
            var cs = new SQLiteConnectionStringBuilder
            {
                DataSource = DbPath,
                Version = 3,
                ForeignKeys = true
            }.ToString();

            using (var conn = new SQLiteConnection(cs))
            {
                conn.Open();

                // 외래키 ON(보수적으로 한 번 더)
                using (var pragma = conn.CreateCommand())
                {
                    pragma.CommandText = "PRAGMA foreign_keys = ON;";
                    pragma.ExecuteNonQuery();
                }

                if (!isNew)
                {
                    // DB가 이미 있으면 여기서 끝 (연결만)
                    return;
                }

                // ====== 신규 DB: 스키마 적용 ======
                if (!File.Exists(SchemaPath))
                    throw new FileNotFoundException($"schema.sql을 찾을 수 없습니다: {SchemaPath}");

                string scriptText = File.ReadAllText(SchemaPath, Encoding.UTF8);

                using (var tx = conn.BeginTransaction())
                {
                    // 대부분의 환경에선 여러 문장을 한 번에 실행해도 됩니다.
                    // 만약 일부 환경에서 한 문장만 실행된다면 아래 SplitExecuteScript를 사용하세요.
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = scriptText;   // 세미콜론(;)으로 끝나는 문장들
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                }
            }
        }
    }


    // 커넥션 팩토리 인터페이스
    public interface ISqliteConnectionFactory
    {
        SQLiteConnection CreateOpen();
    }

    // DB Connection 구현
    public sealed class SqliteConnectionFactory : ISqliteConnectionFactory
    {
        private readonly string _connString;
        public SqliteConnectionFactory(string connString)
        {
            _connString = string.IsNullOrWhiteSpace(connString)
                ? throw new ArgumentException("SQLite connection string is null/empty.")
                : connString;
        }

        public SQLiteConnection CreateOpen()
        {
            var conn = new SQLiteConnection(_connString);
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "PRAGMA foreign_keys = ON;";
                cmd.ExecuteNonQuery();
            }
            return conn;
        }
    }
}
