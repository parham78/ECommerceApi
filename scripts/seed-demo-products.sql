INSERT INTO Products (Name, Price, Stock, Sku, IsActive)
SELECT
    p.Name,
    p.Price,
    p.Stock,
    p.Sku,
    p.IsActive
FROM
(
    VALUES
        ('Headset',          CAST(79.99 AS decimal(18,2)),  18, 'HEADSET-001',      CAST(1 AS bit)),
        ('Webcam',           CAST(69.99 AS decimal(18,2)),  14, 'WEBCAM-001',       CAST(1 AS bit)),
        ('USB-C Hub',        CAST(59.99 AS decimal(18,2)),  25, 'USB-HUB-001',      CAST(1 AS bit)),
        ('External SSD',     CAST(119.99 AS decimal(18,2)), 16, 'SSD-EXT-001',      CAST(1 AS bit)),
        ('Laptop Stand',     CAST(44.99 AS decimal(18,2)),  22, 'LAPTOP-STAND-001', CAST(1 AS bit)),
        ('Desk Lamp',        CAST(39.99 AS decimal(18,2)),  30, 'DESK-LAMP-001',    CAST(1 AS bit)),
        ('USB Microphone',   CAST(89.99 AS decimal(18,2)),  11, 'MICROPHONE-001',   CAST(1 AS bit)),
        ('Desktop Speakers', CAST(64.99 AS decimal(18,2)),  20, 'SPEAKERS-001',     CAST(1 AS bit)),
        ('Game Controller',  CAST(74.99 AS decimal(18,2)),  17, 'CONTROLLER-001',   CAST(1 AS bit)),
        ('Docking Station',  CAST(149.99 AS decimal(18,2)),  9, 'DOCK-001',          CAST(1 AS bit)),
        ('Wireless Charger', CAST(34.99 AS decimal(18,2)),  28, 'CHARGER-WL-001',   CAST(1 AS bit)),
        ('Portable Monitor', CAST(229.99 AS decimal(18,2)),  7, 'MONITOR-PORT-001', CAST(1 AS bit))
) AS p(Name, Price, Stock, Sku, IsActive)
WHERE NOT EXISTS
(
    SELECT 1
    FROM Products existing
    WHERE existing.Sku = p.Sku
);