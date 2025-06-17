START TRANSACTION;

CREATE TABLE `character_items` (
    `id` int NOT NULL AUTO_INCREMENT,
    `characterid` int NOT NULL,
    `itemid` int NOT NULL,
    `amount` smallint NOT NULL,
    `slot` int NOT NULL,
    `isequipped` tinyint(1) NOT NULL,
    `rarity` tinyint unsigned NOT NULL,
    `upgrade` tinyint unsigned NOT NULL,
    CONSTRAINT `PK_character_items` PRIMARY KEY (`id`),
    CONSTRAINT `FK_character_items_characters_characterid` FOREIGN KEY (`characterid`) REFERENCES `characters` (`id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_character_items_characterid` ON `character_items` (`characterid`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250416053227_litacore08_characteritems', '8.0.13');

COMMIT;

