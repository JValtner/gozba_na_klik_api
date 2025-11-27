using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gozba_na_klik.Migrations
{
    /// <inheritdoc />
    public partial class AddLatitudeLongitudeToAddressesIfMissing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE table_name = 'Addresses' 
                        AND column_name = 'Latitude'
                    ) THEN
                        ALTER TABLE ""Addresses"" ADD COLUMN ""Latitude"" double precision NULL;
                    END IF;
                    
                    IF NOT EXISTS (
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE table_name = 'Addresses' 
                        AND column_name = 'Longitude'
                    ) THEN
                        ALTER TABLE ""Addresses"" ADD COLUMN ""Longitude"" double precision NULL;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
