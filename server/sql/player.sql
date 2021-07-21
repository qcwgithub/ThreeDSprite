CREATE TABLE player (
    playerId INT NOT NULL,
    createTime DATETIME,
    
    misc TEXT,

    #region player.sql Auto

    level INT NOT NULL,
    money TEXT,
    diamond INT NOT NULL DEFAULT 0,
    portrait VARCHAR(8),
    userName VARCHAR(32),
    characterConfigId INT NOT NULL,

    #endregion player.sql Auto

    PRIMARY KEY (playerId),
    KEY (userName),
    INDEX (createTime)
);

CREATE TABLE payios (
    rowId BIGINT NOT NULL AUTO_INCREMENT,
    playerId INT NOT NULL,
	env VARCHAR(16) DEFAULT NULL,
    id INT NOT NULL,
	productId VARCHAR(64) NOT NULL,
	bundleId TEXT NOT NULL,
	quantity INT NOT NULL,
    transactionId VARCHAR(64) NOT NULL,
    originalTransactionId VARCHAR(64) DEFAULT NULL,
	purchaseDate DATETIME DEFAULT 0,
	expiresDate DATETIME DEFAULT 0,
	
    time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (rowId),
    INDEX (playerId),
	UNIQUE(playerId, transactionId)
);

CREATE TABLE paylt (
    rowId BIGINT NOT NULL AUTO_INCREMENT,
    orderId VARCHAR(64) NOT NULL,
    playerId INT NOT NULL,
    id INT NOT NULL,
	productId VARCHAR(64) NOT NULL,
	quantity INT NOT NULL,
    fen VARCHAR(16) NOT NULL,
    state TINYINT NOT NULL DEFAULT 0,
    got TINYINT NOT NULL DEFAULT 0,
    createTime DATETIME NOT NULL,
    notifyTime DATETIME,
    gotTime DATETIME,
    orderId3 VARCHAR(64) DEFAULT NULL, #第3方订单号
    time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (rowId),
    INDEX (playerId),
    INDEX (state),
	UNIQUE(orderId)
);

CREATE TABLE payivy (
    rowId BIGINT NOT NULL AUTO_INCREMENT,
    orderId VARCHAR(64) NOT NULL,
    playerId INT NOT NULL,
    id INT NOT NULL,
	productId VARCHAR(64) NOT NULL,
	quantity INT NOT NULL,
    fen VARCHAR(16) NOT NULL,
    state TINYINT NOT NULL DEFAULT 0,
    got TINYINT NOT NULL DEFAULT 0,
    createTime DATETIME NOT NULL,
    notifyTime DATETIME,
    gotTime DATETIME,
    orderId3 VARCHAR(64) DEFAULT NULL, #第3方订单号
    time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    detail TEXT,
    PRIMARY KEY (rowId),
    INDEX (playerId),
    INDEX (state),
	UNIQUE(orderId)
);