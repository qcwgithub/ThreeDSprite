DROP DATABASE IF EXISTS tds_test_player;
CREATE DATABASE tds_test_player;
USE tds_test_player;

DROP USER IF EXISTS user_tds_test_player;
CREATE USER user_tds_test_player IDENTIFIED BY 'gbits*tds_test*player*user*2020';
GRANT SELECT,INSERT,UPDATE,DELETE ON tds_test_player.* TO user_tds_test_player@'%';