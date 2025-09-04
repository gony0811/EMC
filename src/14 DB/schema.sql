PRAGMA foreign_keys = ON;

-- ========== Roles (기존 유지) ==========
CREATE TABLE IF NOT EXISTS Roles (
  RoleId      TEXT NOT NULL PRIMARY KEY,
  Name        TEXT NOT NULL,
  Description TEXT,
  Password    TEXT NOT NULL,
  IsActive    INTEGER NOT NULL DEFAULT 1,
  CreatedAt   TEXT NOT NULL,
  UpdatedAt   TEXT,
  CHECK (length(RoleId) <= 50),
  CHECK (length(Name)   <= 100),
  CHECK (Description IS NULL OR length(Description) <= 200),
  CHECK (length(Password) <= 200),
  CHECK (IsActive IN (0,1))
);

-- ========== PermissionCategory ==========
CREATE TABLE IF NOT EXISTS PermissionCategory (
  CategoryId   INTEGER PRIMARY KEY AUTOINCREMENT,
  Name         TEXT NOT NULL UNIQUE,   -- 예) '권한 설정(작업자)' ...
  DisplayOrder INTEGER NOT NULL DEFAULT 0
);

-- ========== Permission (값 포함: IsEnabled) ==========
CREATE TABLE IF NOT EXISTS Permission (
  PermissionId INTEGER PRIMARY KEY AUTOINCREMENT,
  CategoryId   INTEGER NOT NULL,
  Name         TEXT NOT NULL,
  Description  TEXT,
  IsEnabled    INTEGER NOT NULL DEFAULT 0,  -- 0/1: 기본값 0
  FOREIGN KEY (CategoryId) REFERENCES PermissionCategory(CategoryId)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CHECK (IsEnabled IN (0,1))
);
CREATE UNIQUE INDEX IF NOT EXISTS UX_Permission_Category_Name
  ON Permission(CategoryId, Name);

  -- Role이 관리(편집) 가능한 카테고리 매핑
CREATE TABLE IF NOT EXISTS RoleCategoryManage (
  RoleId     TEXT    NOT NULL,
  CategoryId INTEGER NOT NULL,
  CanManage  INTEGER NOT NULL DEFAULT 1,  -- 0/1
  PRIMARY KEY(RoleId, CategoryId),
  FOREIGN KEY(RoleId)     REFERENCES Roles(RoleId)               ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY(CategoryId) REFERENCES PermissionCategory(CategoryId) ON DELETE CASCADE ON UPDATE CASCADE,
  CHECK (CanManage IN (0,1)),
  CHECK (length(RoleId) <= 50)
);

CREATE INDEX IF NOT EXISTS IX_RoleCategoryManage_RoleId ON RoleCategoryManage(RoleId);
CREATE INDEX IF NOT EXISTS IX_RoleCategoryManage_CategoryId ON RoleCategoryManage(CategoryId);

-- ========== 기본 Roles 시드 ==========
INSERT OR IGNORE INTO Roles(RoleId, Name, Description, Password, IsActive, CreatedAt, UpdatedAt) VALUES
('OPERATOR','작업자','일반 작업자 권한','',1,strftime('%Y-%m-%dT%H:%M:%SZ','now'),NULL),
('ENGINEER','엔지니어','엔지니어 권한','',1,strftime('%Y-%m-%dT%H:%M:%SZ','now'),NULL),
('ADMIN','관리자','관리자 권한','',1,strftime('%Y-%m-%dT%H:%M:%SZ','now'),NULL),
('SERVICE_ENGINEER','서비스 엔지니어','서비스 엔지니어 권한','',1,strftime('%Y-%m-%dT%H:%M:%SZ','now'),NULL);

-- ========== 카테고리 시드 ==========
INSERT OR IGNORE INTO PermissionCategory(Name, DisplayOrder) VALUES
('권한 설정(작업자)', 10),
('권한 설정(엔지니어)', 20),
('사용자 옵션',       30),
('Service Engineer Option', 40);

-- ========== 권한 시드: 권한 설정(작업자) [기본 0] ==========
INSERT OR IGNORE INTO Permission (CategoryId, Name, Description, IsEnabled)
SELECT pc.CategoryId, v.Name, v.Description, 0
FROM PermissionCategory pc
JOIN (
  SELECT '파라미터 설정화면' AS Name, '작업자 대상 화면 접근'   AS Description
  UNION ALL SELECT '파라미터 변경',     '작업자 대상 값 변경'
  UNION ALL SELECT '로그 화면',         '로그 열람'
  UNION ALL SELECT '에러 목록 화면',    '에러/알람 열람'
  UNION ALL SELECT '수동조작 화면',     '수동조작 UI 접근'
  UNION ALL SELECT '모터화면',          '모터 모니터/제어'
  UNION ALL SELECT '센서화면',          '센서 모니터/제어'
  UNION ALL SELECT '경광등 설정화면',   '경광등(비컨) 설정'
) AS v
WHERE pc.Name = '권한 설정(작업자)';

-- ========== 권한 시드: 권한 설정(엔지니어) [기본 0] ==========
INSERT OR IGNORE INTO Permission (CategoryId, Name, Description, IsEnabled)
SELECT pc.CategoryId, v.Name, v.Description, 0
FROM PermissionCategory pc
JOIN (
  SELECT '파라미터 설정화면' AS Name, '엔지니어 대상 화면 접근'   AS Description
  UNION ALL SELECT '파라미터 변경',     '엔지니어 대상 값 변경'
  UNION ALL SELECT '로그 화면',         '로그 열람'
  UNION ALL SELECT '에러 목록 화면',    '에러/알람 열람'
  UNION ALL SELECT '수동조작 화면',     '수동조작 UI 접근'
  UNION ALL SELECT '모터화면',          '모터 모니터/제어'
  UNION ALL SELECT '센서화면',          '센서 모니터/제어'
  UNION ALL SELECT '경광등 설정화면',   '경광등(비컨) 설정'
) AS v
WHERE pc.Name = '권한 설정(엔지니어)';

-- ========== 권한 시드: 사용자 옵션 (테스트 모드 3종) ==========
INSERT OR IGNORE INTO Permission (CategoryId, Name, Description, IsEnabled)
SELECT pc.CategoryId, v.Name, v.Description, 0
FROM PermissionCategory pc
JOIN (
  SELECT 'Door'           AS Name, '문(도어) 사용'              AS Description
  UNION ALL SELECT 'Buzzer'         AS Name, '버저 사용'                  AS Description
  UNION ALL SELECT '테스트 모드 A'  AS Name, '테스트 모드 유형 A'         AS Description
  UNION ALL SELECT '테스트 모드 B'  AS Name, '테스트 모드 유형 B'         AS Description
  UNION ALL SELECT '테스트 모드 C'  AS Name, '테스트 모드 유형 C'         AS Description
) AS v
WHERE pc.Name = '사용자 옵션';

-- ========== 권한 시드: Service Engineer Option ==========
INSERT OR IGNORE INTO Permission (CategoryId, Name, Description, IsEnabled)
SELECT pc.CategoryId, v.Name, v.Description, 0
FROM PermissionCategory pc
JOIN (
  SELECT '드라이런' AS Name, '장비 구동 없이 시나리오 실행' AS Description
) AS v
WHERE pc.Name = 'Service Engineer Option';

-- ========== 테스트 모드 상호배타 트리거 (Permission 표에서 전역으로 보장) ==========
CREATE TRIGGER IF NOT EXISTS trg_TestMode_Exclusive_Update
AFTER UPDATE OF IsEnabled ON Permission
FOR EACH ROW
WHEN NEW.IsEnabled = 1
 AND NEW.PermissionId IN (
   SELECT p.PermissionId
   FROM Permission p
   JOIN PermissionCategory pc ON pc.CategoryId = p.CategoryId
   WHERE pc.Name='사용자 옵션'
     AND p.Name IN ('테스트 모드 A','테스트 모드 B','테스트 모드 C')
 )
BEGIN
  UPDATE Permission
  SET IsEnabled = 0
  WHERE CategoryId = NEW.CategoryId
    AND Name IN ('테스트 모드 A','테스트 모드 B','테스트 모드 C')
    AND PermissionId <> NEW.PermissionId;
END;

CREATE TRIGGER IF NOT EXISTS trg_TestMode_Exclusive_Insert
AFTER INSERT ON Permission
FOR EACH ROW
WHEN NEW.IsEnabled = 1
 AND NEW.PermissionId IN (
   SELECT p.PermissionId
   FROM Permission p
   JOIN PermissionCategory pc ON pc.CategoryId = p.CategoryId
   WHERE pc.Name='사용자 옵션'
     AND p.Name IN ('테스트 모드 A','테스트 모드 B','테스트 모드 C')
 )
BEGIN
  UPDATE Permission
  SET IsEnabled = 0
  WHERE CategoryId = NEW.CategoryId
    AND Name IN ('테스트 모드 A','테스트 모드 B','테스트 모드 C')
    AND PermissionId <> NEW.PermissionId;
END;


-- 매핑 시드:
-- 엔지니어: '권한 설정(작업자)' 관리
INSERT OR IGNORE INTO RoleCategoryManage(RoleId, CategoryId, CanManage)
SELECT 'ENGINEER', pc.CategoryId, 1
FROM PermissionCategory pc WHERE pc.Name='권한 설정(작업자)';

-- 관리자: 작업자/엔지니어 권한 + 사용자 옵션 관리
INSERT OR IGNORE INTO RoleCategoryManage(RoleId, CategoryId, CanManage)
SELECT 'ADMIN', pc.CategoryId, 1
FROM PermissionCategory pc
WHERE pc.Name IN ('권한 설정(작업자)','권한 설정(엔지니어)','사용자 옵션');

-- 서비스 엔지니어: 작업자/엔지니어 권한 + 사용자 옵션 + Service Engineer Option 관리
INSERT OR IGNORE INTO RoleCategoryManage(RoleId, CategoryId, CanManage)
SELECT 'SERVICE_ENGINEER', pc.CategoryId, 1
FROM PermissionCategory pc
WHERE pc.Name IN ('권한 설정(작업자)','권한 설정(엔지니어)','사용자 옵션','Service Engineer Option');