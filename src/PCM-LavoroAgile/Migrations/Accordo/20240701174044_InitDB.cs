using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCM_LavoroAgile.Migrations.Accordo
{
    /// <inheritdoc />
    public partial class InitDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accordi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdDipendente = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NomeCognome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataDiNascita = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Sesso = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LuogoDiNascita = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodiceFiscale = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PosizioneGiuridica = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoriaFasciaRetributiva = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StrutturaUfficioServizio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UidStrutturaUfficioServizio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdCapoStruttura = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CapoStruttura = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailCapoStruttura = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdCapoIntermedio = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CapoIntermedio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailCapoIntermedio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdResponsabileAccordo = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResponsabileAccordo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailResponsabileAccordo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdDirigenteResponsabile = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DirigenteResponsabile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailDirigenteResponsabile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumLivelliStruttura = table.Column<int>(type: "int", nullable: false),
                    IdReferenteInterno = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReferenteInterno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailReferenteInterno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataPresaServizio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priorita_1 = table.Column<bool>(type: "bit", nullable: false),
                    Priorita_2 = table.Column<bool>(type: "bit", nullable: false),
                    Codice = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrimoAccordo = table.Column<bool>(type: "bit", nullable: false),
                    DataFineAccordoPrecedente = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValutazioneEsitiAccordoPrecedente = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataInizioUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFineUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modalita = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PianificazioneGiorniAccordo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DerogaPianificazioneDate = table.Column<bool>(type: "bit", nullable: false),
                    PianificazioneDateAccordo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FasceDiContattabilita = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StrumentazioneUtilizzata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormazioneLavoroAgile = table.Column<bool>(type: "bit", nullable: false),
                    SaluteESicurezza = table.Column<bool>(type: "bit", nullable: false),
                    AccessoVPN = table.Column<bool>(type: "bit", nullable: false),
                    PrivacyEConsensoTrattamentoDati = table.Column<bool>(type: "bit", nullable: false),
                    Stato = table.Column<int>(type: "int", nullable: false),
                    NoteRefereteInterno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VistoSegreteriaTecnica = table.Column<bool>(type: "bit", nullable: false),
                    NoteSegreteriaTecnica = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoteCondivise = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvioNotificaNoteCondivise = table.Column<bool>(type: "bit", nullable: false),
                    NotaDipendente = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataNotaDipendente = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NotaDirigente = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataNotaDirigente = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isValutazionePositiva = table.Column<bool>(type: "bit", nullable: false),
                    DataRecesso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GiustificatoMotivoDiRecesso = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RevisioneAccordo = table.Column<bool>(type: "bit", nullable: false),
                    CodiceComunicazioneMinisteroLavoro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CREATION_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EDIT_TIME = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChildId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accordi", x => x.Id);
                    table.UniqueConstraint("AK_Accordi_Codice", x => x.Codice);
                });

            migrationBuilder.CreateTable(
                name: "Valutazione",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccordoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoteDipendente = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataNoteDipendente = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoteDirigente = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataNoteDirigente = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isApprovata = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Valutazione", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AttivitaAccordo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccordoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    Attivita = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Risultati = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DenominazioneIndicatore = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipologiaIndicatore = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperatoreLogicoIndicatoreTesto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TestoTarget = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TestoTargetRaggiunto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperatoreLogicoIndicatoreNumeroAssoluto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroAssolutoTarget = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroAssolutoRaggiunto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroAssolutoDaTarget = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroAssolutoATarget = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PercentualeIndicatoreDenominazioneNumeratore = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PercentualeIndicatoreDenominazioneDenominatore = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperatoreLogicoIndicatorePercentuale = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PercentualeTarget = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PercentualeDenominatoreTargetRaggiunto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PercentualeNumeratoreTargetRaggiunto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PercentualeTargetRaggiunto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PercentualeDaTarget = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PercentualeATarget = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperatoreLogicoIndicatoreData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataTarget = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataTargetRaggiunto = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataDaTarget = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataATarget = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttivitaAccordo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttivitaAccordo_Accordi_AccordoId",
                        column: x => x.AccordoId,
                        principalTable: "Accordi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoricoStato",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Stato = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Autore = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccordoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoricoStato", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoricoStato_Accordi_AccordoId",
                        column: x => x.AccordoId,
                        principalTable: "Accordi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransmissionStatus",
                columns: table => new
                {
                    AccordoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkingDaysSentSuccessfully = table.Column<bool>(type: "bit", nullable: false),
                    LastWorkingDaysSentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WorkingDaysSendError = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkingActivitiesSentSuccessfully = table.Column<bool>(type: "bit", nullable: false),
                    LastWorkingActivitiesSentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WorkingActivitiesSendError = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NuovaComunicazioneMinisteroLavoroSentSuccessfully = table.Column<bool>(type: "bit", nullable: false),
                    NuovaComunicazioneMinisteroLavoroLastSentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NuovaComunicazioneMinisteroLavoroSendError = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecessoComunicazioneMinisteroLavoroSentSuccessfully = table.Column<bool>(type: "bit", nullable: false),
                    RecessoComunicazioneMinisteroLavoroLastSentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecessoComunicazioneMinisteroLavoroSendError = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransmissionStatus", x => x.AccordoId);
                    table.ForeignKey(
                        name: "FK_TransmissionStatus_Accordi_AccordoId",
                        column: x => x.AccordoId,
                        principalTable: "Accordi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttivitaAccordo_AccordoId",
                table: "AttivitaAccordo",
                column: "AccordoId");

            migrationBuilder.CreateIndex(
                name: "IX_StoricoStato_AccordoId",
                table: "StoricoStato",
                column: "AccordoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttivitaAccordo");

            migrationBuilder.DropTable(
                name: "StoricoStato");

            migrationBuilder.DropTable(
                name: "TransmissionStatus");

            migrationBuilder.DropTable(
                name: "Valutazione");

            migrationBuilder.DropTable(
                name: "Accordi");
        }
    }
}
