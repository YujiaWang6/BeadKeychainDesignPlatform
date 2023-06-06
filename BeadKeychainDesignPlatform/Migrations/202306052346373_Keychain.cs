namespace BeadKeychainDesignPlatform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Keychain : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Keychains",
                c => new
                    {
                        KeychainId = c.Int(nullable: false, identity: true),
                        KeychainName = c.String(),
                    })
                .PrimaryKey(t => t.KeychainId);
            
            CreateTable(
                "dbo.KeychainBeads",
                c => new
                    {
                        Keychain_KeychainId = c.Int(nullable: false),
                        Bead_BeadId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Keychain_KeychainId, t.Bead_BeadId })
                .ForeignKey("dbo.Keychains", t => t.Keychain_KeychainId, cascadeDelete: true)
                .ForeignKey("dbo.Beads", t => t.Bead_BeadId, cascadeDelete: true)
                .Index(t => t.Keychain_KeychainId)
                .Index(t => t.Bead_BeadId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KeychainBeads", "Bead_BeadId", "dbo.Beads");
            DropForeignKey("dbo.KeychainBeads", "Keychain_KeychainId", "dbo.Keychains");
            DropIndex("dbo.KeychainBeads", new[] { "Bead_BeadId" });
            DropIndex("dbo.KeychainBeads", new[] { "Keychain_KeychainId" });
            DropTable("dbo.KeychainBeads");
            DropTable("dbo.Keychains");
        }
    }
}
