START TRANSACTION;

CREATE TABLE `maps` (
    `id` smallint NOT NULL AUTO_INCREMENT,
    `name` longtext CHARACTER SET utf8mb4 NULL,
    `bgm` int NOT NULL,
    `isshopallowed` tinyint(1) NOT NULL,
    `mapgrid` longblob NOT NULL,
    `exprate` tinyint unsigned NOT NULL,
    `goldrate` tinyint unsigned NOT NULL,
    `droprate` tinyint unsigned NOT NULL,
    `ispvpallowed` tinyint(1) NOT NULL,
    `height` smallint NOT NULL,
    `width` smallint NOT NULL,
    CONSTRAINT `PK_maps` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250413155938_litacore03_maps', '8.0.13');

COMMIT;

