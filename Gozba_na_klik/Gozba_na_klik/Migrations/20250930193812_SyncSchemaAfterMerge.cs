using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gozba_na_klik.Migrations
{
    /// <inheritdoc />
    public partial class SyncSchemaAfterMerge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add column only if it doesn't exist
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1
                        FROM information_schema.columns
                        WHERE table_name='ClosedDates' AND column_name='Reason'
                    ) THEN
                        ALTER TABLE ""ClosedDates"" ADD COLUMN ""Reason"" text;
                    END IF;
                END
                $$;
            ");

            // Ensure existing rows have NULL (or a default value)
            migrationBuilder.Sql(@"
                UPDATE ""ClosedDates""
                SET ""Reason"" = NULL
                WHERE ""Reason"" IS NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop column only if it exists
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM information_schema.columns
                        WHERE table_name='ClosedDates' AND column_name='Reason'
                    ) THEN
                        ALTER TABLE ""ClosedDates"" DROP COLUMN ""Reason"";
                    END IF;
                END
                $$;
            ");
        }
    }
}
