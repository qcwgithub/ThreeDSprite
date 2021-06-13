CREATE TABLE account (
    rowId BIGINT NOT NULL AUTO_INCREMENT,
    platform VARCHAR(16) NOT NULL DEFAULT '',
    channel VARCHAR(32) NOT NULL DEFAULT '',
    channelUserId VARCHAR(64) NOT NULL DEFAULT '',
    playerId INT NOT NULL,
    createTime DATETIME,
    isBan BOOLEAN NOT NULL DEFAULT FALSE,
    unbanTime DATETIME,
    banReason TEXT,
	oaid VARCHAR(64),
	imei VARCHAR(64),
    userInfo TEXT,
    PRIMARY KEY (rowId),
    UNIQUE(channel, channelUserId),
    INDEX(playerId)
);

CREATE TABLE player_id (
    id INT,
    playerId INT,
    PRIMARY KEY (id)
);

INSERT INTO player_id (id, playerId) VALUES (1, 1000);