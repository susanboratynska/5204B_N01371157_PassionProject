namespace PassionProject_SusanBoratynska.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Feb17 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "hasPic", c => c.Int(nullable: true));
            AddColumn("dbo.Players", "picExtension", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.Players", "picExtension");
            DropColumn("dbo.Players", "hasPic");
        }
    }
}

