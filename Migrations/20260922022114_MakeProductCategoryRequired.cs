using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class MakeProductCategoryRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Categories WHERE Slug = 'workspace')
                    INSERT INTO Categories (Name, Slug)
                    VALUES ('Workspace', 'workspace');

                IF NOT EXISTS (SELECT 1 FROM Categories WHERE Slug = 'displays')
                    INSERT INTO Categories (Name, Slug)
                    VALUES ('Displays', 'displays');

                IF NOT EXISTS (SELECT 1 FROM Categories WHERE Slug = 'audio-video')
                    INSERT INTO Categories (Name, Slug)
                    VALUES ('Audio & Video', 'audio-video');

                IF NOT EXISTS (SELECT 1 FROM Categories WHERE Slug = 'connectivity-power')
                    INSERT INTO Categories (Name, Slug)
                    VALUES ('Connectivity & Power', 'connectivity-power');

                IF NOT EXISTS (SELECT 1 FROM Categories WHERE Slug = 'storage-accessories')
                    INSERT INTO Categories (Name, Slug)
                    VALUES ('Storage & Accessories', 'storage-accessories');


                UPDATE p
                SET CategoryId = c.Id
                FROM Products p
                JOIN Categories c ON c.Slug = 'workspace'
                WHERE p.Sku IN (
                    'LEGACY-1',
                    'LEGACY-4',
                    'LAPTOP-STAND-001',
                    'DESK-LAMP-001'
                );

                UPDATE p
                SET CategoryId = c.Id
                FROM Products p
                JOIN Categories c ON c.Slug = 'displays'
                WHERE p.Sku IN (
                    'MONITOR-GAMING-001',
                    'MONITOR-PORT-001'
                );

                UPDATE p
                SET CategoryId = c.Id
                FROM Products p
                JOIN Categories c ON c.Slug = 'audio-video'
                WHERE p.Sku IN (
                    'HEADSET-001',
                    'WEBCAM-001',
                    'MICROPHONE-001',
                    'SPEAKERS-001'
                );

                UPDATE p
                SET CategoryId = c.Id
                FROM Products p
                JOIN Categories c ON c.Slug = 'connectivity-power'
                WHERE p.Sku IN (
                    'USB-HUB-001',
                    'DOCK-001',
                    'CHARGER-WL-001'
                );

                UPDATE p
                SET CategoryId = c.Id
                FROM Products p
                JOIN Categories c ON c.Slug = 'storage-accessories'
                WHERE p.Sku IN (
                    'SSD-EXT-001',
                    'CONTROLLER-001'
                );
            ");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Products",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}