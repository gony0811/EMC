# 새 마이그레이션 생성
dotnet ef migrations add InitialCreate --output-dir Migrations

# DB에 적용(업데이트)
dotnet ef database update

# 특정 마이그레이션까지 롤백/적용
dotnet ef database update 20240912000123_InitialCreate

# 마이그레이션 목록 보기
dotnet ef migrations list

# 마지막 마이그레이션 취소
dotnet ef migrations remove

# 마이그레이션 스크립트 생성(배포용 SQL)
dotnet ef migrations script