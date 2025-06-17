START TRANSACTION;

CREATE TABLE `channels` (
    `id` int NOT NULL AUTO_INCREMENT,
    `ipaddress` longtext CHARACTER SET utf8mb4 NOT NULL,
    `channelport` int NOT NULL,
    `maxplayers` int NOT NULL,
    `onlineplayers` int NOT NULL,
    `channelstatus` int NOT NULL,
    CONSTRAINT `PK_channels` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250419003539_litacore11_channels', '8.0.13');

COMMIT;

