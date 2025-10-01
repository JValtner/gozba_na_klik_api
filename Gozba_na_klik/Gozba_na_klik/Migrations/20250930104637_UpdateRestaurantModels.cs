using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gozba_na_klik.Migrations
{
    public partial class UpdateRestaurantModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Use SQL to conditionally add columns only if they do not exist
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                                   WHERE table_name='Restaurants' AND column_name='Address') THEN
                        ALTER TABLE ""Restaurants"" ADD ""Address"" text;
                    END IF;

                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                                   WHERE table_name='Restaurants' AND column_name='Description') THEN
                        ALTER TABLE ""Restaurants"" ADD ""Description"" text;
                    END IF;

                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                                   WHERE table_name='Restaurants' AND column_name='Phone') THEN
                        ALTER TABLE ""Restaurants"" ADD ""Phone"" text;
                    END IF;

                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                                   WHERE table_name='ClosedDates' AND column_name='Reason') THEN
                        ALTER TABLE ""ClosedDates"" ADD ""Reason"" text;
                    END IF;
                END
                $$;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop columns safely (PostgreSQL ignores if column doesn't exist)
            migrationBuilder.Sql(@"
                ALTER TABLE ""Restaurants"" DROP COLUMN IF EXISTS ""Address"";
                ALTER TABLE ""Restaurants"" DROP COLUMN IF EXISTS ""Description"";
                ALTER TABLE ""Restaurants"" DROP COLUMN IF EXISTS ""Phone"";
                ALTER TABLE ""ClosedDates"" DROP COLUMN IF EXISTS ""Reason"";
            ");
        }
    }
}
