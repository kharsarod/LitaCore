START TRANSACTION;

CREATE TABLE `session_tokens` (
    `id` bigint NOT NULL AUTO_INCREMENT,
    `username` longtext CHARACTER SET utf8mb4 NULL,
    `token` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_session_tokens` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250414041100_litacore04', '8.0.13');

COMMIT;

