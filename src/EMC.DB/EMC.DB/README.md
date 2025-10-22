# 새 마이그레이션 추가하는 방법 ( 주의: 기존 데이터가 날아갈 수 있음. 기본 데이터 및 필수 데이터들은 Config/DbSeeder.cs 파일에 정의하세요. )
1. Domain 폴더 내에 Entity Class 생성

2. Config/DataConfig.cs 파일에 Entity Config 추가

3. ( 추천 ) Migrations 폴더 삭제 및 db 파일 삭제 - 이유: Sqlite는 ALTER TABLE 기능이 없음. 이 때 기존 데이터들이 모두 삭제됨
	-  Migrations 폴더 위치 EMC.DB 하위 
	-  emc.db 파일 위치 : 현재 프로그램 exe 파일 위치와 동일

4. 아래에 명령어를 수행하세요 (MigrationFileName 부분에 원하는 마이그레이션 파일 이름을 입력하세요.)

	- [ 패키지 관리자 콘솔 명령어 ] Add-Migration MigrationFileName -Project EMC.DB -StartupProject EMC
	- [ PowerShell 명령어 ] dotnet ef migrations add MigrationFileName --project EMC.DB --startup-project EMC

4. 빌드 후 프로그램 실행


# Repository 패턴 사용
설명 링크: https://www.notion.so/DBRepository-2948217ad33a80c09463eddad7e21e8e