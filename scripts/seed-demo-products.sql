/* ============================================================
   Supply demo catalog seed
   - Creates categories if missing
   - Creates products if SKU does not already exist
   - Safe to run multiple times
   ============================================================ */


/* ---------- Categories ---------- */

IF NOT EXISTS (SELECT 1 FROM Categories WHERE Slug = 'workspace')
BEGIN
    INSERT INTO Categories (Name, Slug)
    VALUES ('Workspace', 'workspace');
END;

IF NOT EXISTS (SELECT 1 FROM Categories WHERE Slug = 'displays')
BEGIN
    INSERT INTO Categories (Name, Slug)
    VALUES ('Displays', 'displays');
END;

IF NOT EXISTS (SELECT 1 FROM Categories WHERE Slug = 'audio-video')
BEGIN
    INSERT INTO Categories (Name, Slug)
    VALUES ('Audio & Video', 'audio-video');
END;

IF NOT EXISTS (SELECT 1 FROM Categories WHERE Slug = 'connectivity-power')
BEGIN
    INSERT INTO Categories (Name, Slug)
    VALUES ('Connectivity & Power', 'connectivity-power');
END;

IF NOT EXISTS (SELECT 1 FROM Categories WHERE Slug = 'storage-accessories')
BEGIN
    INSERT INTO Categories (Name, Slug)
    VALUES ('Storage & Accessories', 'storage-accessories');
END;


/* ---------- Products ---------- */

INSERT INTO Products
(
    Name,
    Price,
    Stock,
    Sku,
    IsActive,
    CategoryId
)
SELECT
    p.Name,
    p.Price,
    p.Stock,
    p.Sku,
    p.IsActive,
    c.Id
FROM
(
    VALUES

        /* Workspace */
        (
            'Keyboard',
            CAST(99.99 AS decimal(18,2)),
            25,
            'LEGACY-1',
            CAST(1 AS bit),
            'workspace'
        ),
        (
            'Mouse',
            CAST(49.99 AS decimal(18,2)),
            30,
            'LEGACY-4',
            CAST(1 AS bit),
            'workspace'
        ),
        (
            'Laptop Stand',
            CAST(44.99 AS decimal(18,2)),
            22,
            'LAPTOP-STAND-001',
            CAST(1 AS bit),
            'workspace'
        ),
        (
            'Desk Lamp',
            CAST(39.99 AS decimal(18,2)),
            30,
            'DESK-LAMP-001',
            CAST(1 AS bit),
            'workspace'
        ),
        (
            'Ergonomic Wrist Rest',
            CAST(29.99 AS decimal(18,2)),
            24,
            'WRIST-REST-001',
            CAST(1 AS bit),
            'workspace'
        ),


        /* Displays */
        (
            'Gaming Monitor',
            CAST(279.99 AS decimal(18,2)),
            12,
            'MONITOR-GAMING-001',
            CAST(1 AS bit),
            'displays'
        ),
        (
            'Portable Monitor',
            CAST(229.99 AS decimal(18,2)),
            7,
            'MONITOR-PORT-001',
            CAST(1 AS bit),
            'displays'
        ),
        (
            '27-inch 4K Monitor',
            CAST(399.99 AS decimal(18,2)),
            10,
            'MONITOR-4K-001',
            CAST(1 AS bit),
            'displays'
        ),
        (
            'Ultrawide Monitor',
            CAST(449.99 AS decimal(18,2)),
            8,
            'MONITOR-UW-001',
            CAST(1 AS bit),
            'displays'
        ),
        (
            'Monitor Arm',
            CAST(89.99 AS decimal(18,2)),
            15,
            'MONITOR-ARM-001',
            CAST(1 AS bit),
            'displays'
        ),


        /* Audio & Video */
        (
            'Headset',
            CAST(79.99 AS decimal(18,2)),
            18,
            'HEADSET-001',
            CAST(1 AS bit),
            'audio-video'
        ),
        (
            'Webcam',
            CAST(69.99 AS decimal(18,2)),
            14,
            'WEBCAM-001',
            CAST(1 AS bit),
            'audio-video'
        ),
        (
            'USB Microphone',
            CAST(89.99 AS decimal(18,2)),
            11,
            'MICROPHONE-001',
            CAST(1 AS bit),
            'audio-video'
        ),
        (
            'Desktop Speakers',
            CAST(64.99 AS decimal(18,2)),
            20,
            'SPEAKERS-001',
            CAST(1 AS bit),
            'audio-video'
        ),
        (
            'Wireless Earbuds',
            CAST(109.99 AS decimal(18,2)),
            16,
            'EARBUDS-WL-001',
            CAST(1 AS bit),
            'audio-video'
        ),


        /* Connectivity & Power */
        (
            'USB-C Hub',
            CAST(59.99 AS decimal(18,2)),
            25,
            'USB-HUB-001',
            CAST(1 AS bit),
            'connectivity-power'
        ),
        (
            'Docking Station',
            CAST(149.99 AS decimal(18,2)),
            9,
            'DOCK-001',
            CAST(1 AS bit),
            'connectivity-power'
        ),
        (
            'Wireless Charger',
            CAST(34.99 AS decimal(18,2)),
            28,
            'CHARGER-WL-001',
            CAST(1 AS bit),
            'connectivity-power'
        ),
        (
            '100W USB-C Charger',
            CAST(79.99 AS decimal(18,2)),
            20,
            'CHARGER-100W-001',
            CAST(1 AS bit),
            'connectivity-power'
        ),
        (
            'Surge Protector',
            CAST(39.99 AS decimal(18,2)),
            26,
            'SURGE-001',
            CAST(1 AS bit),
            'connectivity-power'
        ),


        /* Storage & Accessories */
        (
            'External SSD',
            CAST(119.99 AS decimal(18,2)),
            16,
            'SSD-EXT-001',
            CAST(1 AS bit),
            'storage-accessories'
        ),
        (
            'Game Controller',
            CAST(74.99 AS decimal(18,2)),
            17,
            'CONTROLLER-001',
            CAST(1 AS bit),
            'storage-accessories'
        ),
        (
            'USB Flash Drive',
            CAST(24.99 AS decimal(18,2)),
            35,
            'FLASH-USB-001',
            CAST(1 AS bit),
            'storage-accessories'
        ),
        (
            'SD Card Reader',
            CAST(29.99 AS decimal(18,2)),
            21,
            'CARD-READER-001',
            CAST(1 AS bit),
            'storage-accessories'
        ),
        (
            'Laptop Sleeve',
            CAST(34.99 AS decimal(18,2)),
            24,
            'LAPTOP-SLEEVE-001',
            CAST(1 AS bit),
            'storage-accessories'
        )

) AS p
(
    Name,
    Price,
    Stock,
    Sku,
    IsActive,
    CategorySlug
)

INNER JOIN Categories c
    ON c.Slug = p.CategorySlug

WHERE NOT EXISTS
(
    SELECT 1
    FROM Products existing
    WHERE existing.Sku = p.Sku
);