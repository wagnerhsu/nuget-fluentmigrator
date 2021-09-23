using FluentMigrator;

namespace MigrateCommon
{
    [Migration(20190101080000)]
    public class AddGetAllMember : Migration
    {
        public override void Down()
        {
            Execute.Script(@"Scripts\DropSP_GetAllMember.sql");
        }

        public override void Up()
        {
            Execute.Script(@"Scripts\CreateSP_GetAllMember.sql");
        }
    }
}
