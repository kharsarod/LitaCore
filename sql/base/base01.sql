CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `accounts` (
    `id` int NOT NULL AUTO_INCREMENT,
    `username` longtext CHARACTER SET utf8mb4 NOT NULL,
    `password` longtext CHARACTER SET utf8mb4 NOT NULL,
    `rank` tinyint unsigned NOT NULL,
    `isbanned` tinyint(1) NOT NULL,
    CONSTRAINT `PK_accounts` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `characters` (
    `id` int NOT NULL AUTO_INCREMENT,
    `accountid` int NOT NULL,
    `name` longtext CHARACTER SET utf8mb4 NULL,
    `level` tinyint unsigned NOT NULL,
    `exp` bigint NOT NULL,
    `joblevel` tinyint unsigned NOT NULL,
    `jobexp` bigint NOT NULL,
    `herolevel` tinyint unsigned NOT NULL,
    `heroexp` bigint NOT NULL,
    `class` tinyint unsigned NOT NULL,
    `gender` tinyint unsigned NOT NULL,
    `haircolor` tinyint unsigned NOT NULL,
    `hairstyle` tinyint unsigned NOT NULL,
    `health` int NOT NULL,
    `maxhealth` int NOT NULL,
    `mana` int NOT NULL,
    `maxmana` int NOT NULL,
    `dignity` tinyint unsigned NOT NULL,
    `reputation` int NOT NULL,
    `gold` bigint NOT NULL,
    `compliments` smallint NOT NULL,
    `mapid` smallint NOT NULL,
    `mapposx` smallint NOT NULL,
    `mapposy` smallint NOT NULL,
    `biography` longtext CHARACTER SET utf8mb4 NULL,
    `slot` tinyint unsigned NOT NULL,
    `act4deadcount` int NOT NULL,
    `act4victims` int NOT NULL,
    `act4points` int NOT NULL,
    `isarenachampion` tinyint(1) NOT NULL,
    `isblockedbuff` tinyint(1) NOT NULL,
    `isemoticonblocked` tinyint(1) NOT NULL,
    `isexchangeblocked` tinyint(1) NOT NULL,
    `isfamilyrequestblocked` tinyint(1) NOT NULL,
    `isfriendrequestblocked` tinyint(1) NOT NULL,
    `isgrouprequestblocked` tinyint(1) NOT NULL,
    `isherochatblocked` tinyint(1) NOT NULL,
    `ishealthblocked` tinyint(1) NOT NULL,
    `maxpets` tinyint unsigned NOT NULL,
    `isminilandinviteblocked` tinyint(1) NOT NULL,
    `minilandmsg` longtext CHARACTER SET utf8mb4 NULL,
    `minilandpts` smallint NOT NULL,
    `minilandstate` tinyint(1) NOT NULL,
    `cursoraimlock` tinyint(1) NOT NULL,
    `isquickgetupblocked` tinyint(1) NOT NULL,
    `ragepts` bigint NOT NULL,
    `specialistaddpts` int NOT NULL,
    `specialistpts` int NOT NULL,
    `state` tinyint unsigned NOT NULL,
    `talentarenaloses` int NOT NULL,
    `talentarenasurrender` int NOT NULL,
    `talentarenawins` int NOT NULL,
    `iswhispblocked` tinyint(1) NOT NULL,
    `isdisplayhealthblocked` tinyint(1) NOT NULL,
    `isdisplaycdblocked` tinyint(1) NOT NULL,
    `isblockedhud` tinyint(1) NOT NULL,
    `isblockedhat` tinyint(1) NOT NULL,
    CONSTRAINT `PK_characters` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250412021010_litacore01', '8.0.13');

COMMIT;

