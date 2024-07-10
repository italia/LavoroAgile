using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCM_LavoroAgile.Migrations.Struttura
{
    /// <inheritdoc />
    public partial class InitDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SegreteriaTecnica",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomeCompleto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailUtente = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CREATION_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EDIT_TIME = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChildId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SegreteriaTecnica", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Strutture",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StrutturaCompleta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroLivelli = table.Column<int>(type: "int", nullable: false),
                    StrutturaLiv1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StrutturaLiv2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StrutturaLiv3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DirettaCollaborazione = table.Column<bool>(type: "bit", nullable: false),
                    IdCapoStruttura = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CapoStruttura = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailCapoStruttura = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdCapoIntermedio = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CapoIntermedio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailCapoIntermedio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdDirigenteResponsabile = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DirigenteResponsabile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailDirigenteResponsabile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdResponsabileAccordo = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResponsabileAccordo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailResponsabileAccordo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdReferenteInterno = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReferenteInterno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailReferenteInterno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CREATION_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EDIT_TIME = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChildId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Strutture", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SegreteriaTecnica");

            migrationBuilder.DropTable(
                name: "Strutture");
        }
    }
}
