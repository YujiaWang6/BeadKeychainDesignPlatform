namespace BeadKeychainDesignPlatform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BeadColour : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BeadColours",
                c => new
                    {
                        ColourId = c.Int(nullable: false, identity: true),
                        ColourName = c.String(),
                        ColourProperty = c.String(),
                    })
                .PrimaryKey(t => t.ColourId);
            
            AddColumn("dbo.Beads", "ColourId", c => c.Int(nullable: false));
            CreateIndex("dbo.Beads", "ColourId");
            AddForeignKey("dbo.Beads", "ColourId", "dbo.BeadColours", "ColourId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Beads", "ColourId", "dbo.BeadColours");
            DropIndex("dbo.Beads", new[] { "ColourId" });
            DropColumn("dbo.Beads", "ColourId");
            DropTable("dbo.BeadColours");
        }
    }
}
