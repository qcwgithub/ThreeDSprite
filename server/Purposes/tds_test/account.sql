DROP DATABASE IF EXISTS tds_test_account;
CREATE DATABASE tds_test_account;
USE tds_test_account;

DROP USER IF EXISTS user_tds_test_account;
CREATE USER user_tds_test_account IDENTIFIED BY 'gbits*tds_test*account*user*2020';
GRANT SELECT,INSERT,UPDATE,DELETE ON tds_test_account.* TO user_tds_test_account@'%';