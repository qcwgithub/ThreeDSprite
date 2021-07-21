CREATE TABLE player (
    playerId INT NOT NULL,
    createTime DATETIME,
    
    misc TEXT,

    #region autoSql
    # a
    adReward TEXT, #0/30
    auth TEXT, #1/30

    # b
    badge BIGINT NOT NULL DEFAULT 0, #2/30
    badgeC BIGINT NOT NULL DEFAULT 0, #3/30
    badgeAct INT NOT NULL DEFAULT 0, #4/30
    bank TEXT, #5/30
    battle TEXT, #6/30

    # d
    dailyReward TEXT, #7/30
    diamond INT NOT NULL DEFAULT 0, #8/30

    # g
    giftVoucher INT NOT NULL DEFAULT 0, #10/30

    # l
    loginReward INT NOT NULL DEFAULT 0, #11/30

    # m
    money TEXT, #12/30

    # n
    numbers TEXT, #14/30

    # o
    offlineBonus TEXT, #15/30

    # p
    portrait VARCHAR(8), #16/30

    # s
    statistics TEXT, #17/30
    star INT NOT NULL DEFAULT 0, #18/30
    skinVoucher INT NOT NULL DEFAULT 0, #19/30
    subscribe TEXT, #20/30
    skin TEXT, #21/30

    # t
    task TEXT, #22/30
    totalGameTimeMs BIGINT NOT NULL DEFAULT 0, #23/30
    totalLoginTimes INT NOT NULL DEFAULT 0, #24/30
    town TEXT, #25/30
    tutorial TEXT, #26/30

    # u
    userID VARCHAR(64), #27/30
    userName VARCHAR(32), #28/30

    #endregion autoSql

    PRIMARY KEY (id),
    KEY (userName),
    KEY (userID),
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