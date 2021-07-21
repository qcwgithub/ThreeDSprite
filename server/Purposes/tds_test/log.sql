DROP DATABASE IF EXISTS tds_test_log;
CREATE DATABASE tds_test_log;
USE tds_test_log;

DROP USER IF EXISTS user_tds_test_log;
CREATE USER user_tds_test_log IDENTIFIED BY 'gbits*tds_test*log*user*2020';
GRANT SELECT,INSERT,UPDATE,DELETE ON tds_test_log.* TO user_tds_test_log@'%';