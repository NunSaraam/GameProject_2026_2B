-- 1.테이블 생성
CREATE TABLE `players` (
	`player_id` INT NOT NULL AUTO_INCREMENT,
	`user_name` VARCHAR(50) NOT NULL DEFAULT '0',
	`email` VARCHAR(50) NOT NULL,
	`password_hash` VARCHAR(255) NOT NULL,
	`created_at` TIMESTAMP NOT NULL DEFAULT (CURRENT_TIMESTAMP()),
	`last_login` TIMESTAMP NULL DEFAULT NULL,
	PRIMARY KEY (`player_id`),
	UNIQUE INDEX `user_name` (`user_name`),
	UNIQUE INDEX `email` (`email`)
)
COLLATE='utf8mb4_0900_ai_ci'

-- 2.플레이어 데이터 삽임

INSERT INTO players(user_name, email, password_hash) VALUES ('hero123','hero123@gmail.com','hashhed_password1')


-- 3.여러개 플레이어 데이터 삽임
INSERT INTO players(user_name, email, password_hash) VALUES
('hero234','hero234@gmail.com','hashhed_password2'),
('hero345','hero345@gmail.com','hashhed_password3'),
('hero456','hero456@gmail.com','hashhed_password4'),
('hero567','hero567@gmail.com','hashhed_password5'),
('hero678','hero678@gmail.com','hashhed_password6')

-- 4.유저 이름 기준으로 최근 로그인 보기
SELECT user_name, last_login FROM players

-- 5.특정 플레이어 정보 업데이트
UPDATE players SET last_login = CURRENT_TIMESTAMP WHERE user_name = 'hero123'

-- 6.조건에 맞는 플레이어 검색
SELECT user_name, email FROM players WHERE user_name LIKE '%hero%'

-- 7.플레이어 삭제
DELETE FROM players WHERE user_name = 'hero123'
