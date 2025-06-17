START TRANSACTION;

DROP TABLE IF EXISTS `session_tokens`;

CREATE TABLE `login_sessions` (
    `id` int NOT NULL AUTO_INCREMENT,
    `sessionid` int NOT NULL,
    `expiration` datetime(6) NOT NULL,
    CONSTRAINT `PK_login_sessions` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250417220554_litacore10_login_sessions', '8.0.13');

COMMIT;

