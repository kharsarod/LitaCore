CREATE USER IF NOT EXISTS 'litacore'@'localhost' IDENTIFIED BY 'litacore';

GRANT ALL PRIVILEGES ON litacore_auth.* TO 'litacore'@'localhost';
GRANT ALL PRIVILEGES ON litacore_characters.* TO 'litacore'@'localhost';
GRANT ALL PRIVILEGES ON litacore_world.* TO 'litacore'@'localhost';

FLUSH PRIVILEGES;