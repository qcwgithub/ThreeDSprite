CREATE TABLE player_diamond (
    rowId BIGINT NOT NULL AUTO_INCREMENT,
    playerId INT NOT NULL,
    place INT NOT NULL,
    delta INT NOT NULL,
    current INT NOT NULL,
    level INT NOT NULL,
    i1 INT NOT NULL DEFAULT 0,
    i2 INT NOT NULL DEFAULT 0,
    s1 TEXT DEFAULT NULL,
    s2 TEXT DEFAULT NULL,
    time TIMESTAMP,
    PRIMARY KEY (rowId),
    KEY (playerId),
    INDEX (time)
);

CREATE TABLE player_badge(
    rowId BIGINT NOT NULL AUTO_INCREMENT,
    playerId INT NOT NULL,
    place INT NOT NULL,
    delta INT NOT NULL,
    current INT NOT NULL,
    currentC INT NOT NULL,
    level INT NOT NULL,
    i1 INT NOT NULL DEFAULT 0,
    i2 INT NOT NULL DEFAULT 0,
    s1 TEXT DEFAULT NULL,
    s2 TEXT DEFAULT NULL,
    time TIMESTAMP,
    PRIMARY KEY (rowId),
    KEY (playerId),
    INDEX (time)
);

CREATE TABLE player_giftVoucher (
    rowId BIGINT NOT NULL AUTO_INCREMENT,
    playerId INT NOT NULL,
    place INT NOT NULL,
    delta INT NOT NULL,
    current INT NOT NULL,
    level INT NOT NULL,
    i1 INT NOT NULL DEFAULT 0,
    i2 INT NOT NULL DEFAULT 0,
    s1 TEXT DEFAULT NULL,
    s2 TEXT DEFAULT NULL,
    time TIMESTAMP,
    PRIMARY KEY (rowId),
    KEY (playerId),
    INDEX (time)
);

CREATE TABLE player_login (
    rowId BIGINT NOT NULL AUTO_INCREMENT,
    playerId INT NOT NULL,
    level INT NOT NULL,
	uploadProfile BOOLEAN NOT NULL DEFAULT FALSE,
    time TIMESTAMP,
    PRIMARY KEY (rowId),
    KEY (playerId),
    INDEX (time)
);

CREATE TABLE player_logout (
    rowId BIGINT NOT NULL AUTO_INCREMENT,
    playerId INT NOT NULL,
    level INT NOT NULL,
    time TIMESTAMP,
    PRIMARY KEY (rowId),
    KEY (playerId),
    INDEX (time)
);

CREATE TABLE player_houseLevel(
    rowId BIGINT NOT NULL AUTO_INCREMENT,
    playerId INT NOT NULL,
    level INT NOT NULL,
    gameTimeS INT DEFAULT 0,
    time TIMESTAMP,
    PRIMARY KEY (rowId),
    KEY (playerId),
    INDEX (time)
);

CREATE TABLE player_partyLevel(
    rowId BIGINT NOT NULL AUTO_INCREMENT,
    playerId INT NOT NULL,
    level INT NOT NULL,
    partyLevel INT NOT NULL,
    time TIMESTAMP,
    PRIMARY KEY (rowId),
    KEY (playerId),
    INDEX (time)
);

CREATE TABLE player_changeName (
    rowId BIGINT NOT NULL AUTO_INCREMENT,
    playerId INT NOT NULL,
    level INT NOT NULL,
    userName VARCHAR(32),
    time TIMESTAMP,
    PRIMARY KEY (rowId),
    KEY (playerId),
    INDEX (time)
);

CREATE TABLE account_changeChannel (
    rowId BIGINT NOT NULL AUTO_INCREMENT,
    playerId INT NOT NULL,
    channel1 VARCHAR(32) NOT NULL DEFAULT '',
    channelUserId1 VARCHAR(64) NOT NULL DEFAULT '',
    channel2 VARCHAR(32) NOT NULL DEFAULT '',
    channelUserId2 VARCHAR(64) NOT NULL DEFAULT '',
    time TIMESTAMP,
    PRIMARY KEY (rowId),
    KEY (playerId),
    INDEX (time)
);