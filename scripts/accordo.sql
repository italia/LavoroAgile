IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240701174044_InitDB'
)
BEGIN
    CREATE TABLE [Accordi] (
        [Id] uniqueidentifier NOT NULL,
        [IdDipendente] uniqueidentifier NULL,
        [NomeCognome] nvarchar(max) NULL,
        [DataDiNascita] datetime2 NULL,
        [Sesso] nvarchar(max) NULL,
        [LuogoDiNascita] nvarchar(max) NULL,
        [CodiceFiscale] nvarchar(max) NULL,
        [Email] nvarchar(max) NULL,
        [PosizioneGiuridica] nvarchar(max) NULL,
        [CategoriaFasciaRetributiva] nvarchar(max) NULL,
        [StrutturaUfficioServizio] nvarchar(max) NULL,
        [UidStrutturaUfficioServizio] nvarchar(max) NULL,
        [IdCapoStruttura] uniqueidentifier NULL,
        [CapoStruttura] nvarchar(max) NULL,
        [EmailCapoStruttura] nvarchar(max) NULL,
        [IdCapoIntermedio] uniqueidentifier NULL,
        [CapoIntermedio] nvarchar(max) NULL,
        [EmailCapoIntermedio] nvarchar(max) NULL,
        [IdResponsabileAccordo] uniqueidentifier NULL,
        [ResponsabileAccordo] nvarchar(max) NULL,
        [EmailResponsabileAccordo] nvarchar(max) NULL,
        [IdDirigenteResponsabile] uniqueidentifier NULL,
        [DirigenteResponsabile] nvarchar(max) NULL,
        [EmailDirigenteResponsabile] nvarchar(max) NULL,
        [NumLivelliStruttura] int NOT NULL,
        [IdReferenteInterno] uniqueidentifier NULL,
        [ReferenteInterno] nvarchar(max) NULL,
        [EmailReferenteInterno] nvarchar(max) NULL,
        [DataPresaServizio] datetime2 NULL,
        [Telefono] nvarchar(max) NULL,
        [Priorita_1] bit NOT NULL,
        [Priorita_2] bit NOT NULL,
        [Codice] int NOT NULL IDENTITY,
        [PrimoAccordo] bit NOT NULL,
        [DataFineAccordoPrecedente] datetime2 NULL,
        [ValutazioneEsitiAccordoPrecedente] nvarchar(max) NULL,
        [DataInizioUtc] datetime2 NOT NULL,
        [DataFineUtc] datetime2 NOT NULL,
        [Modalita] nvarchar(max) NULL,
        [PianificazioneGiorniAccordo] nvarchar(max) NULL,
        [DerogaPianificazioneDate] bit NOT NULL,
        [PianificazioneDateAccordo] nvarchar(max) NULL,
        [FasceDiContattabilita] nvarchar(max) NULL,
        [StrumentazioneUtilizzata] nvarchar(max) NULL,
        [FormazioneLavoroAgile] bit NOT NULL,
        [SaluteESicurezza] bit NOT NULL,
        [AccessoVPN] bit NOT NULL,
        [PrivacyEConsensoTrattamentoDati] bit NOT NULL,
        [Stato] int NOT NULL,
        [NoteRefereteInterno] nvarchar(max) NULL,
        [VistoSegreteriaTecnica] bit NOT NULL,
        [NoteSegreteriaTecnica] nvarchar(max) NULL,
        [NoteCondivise] nvarchar(max) NULL,
        [InvioNotificaNoteCondivise] bit NOT NULL,
        [NotaDipendente] nvarchar(max) NULL,
        [DataNotaDipendente] datetime2 NULL,
        [NotaDirigente] nvarchar(max) NULL,
        [DataNotaDirigente] datetime2 NULL,
        [isValutazionePositiva] bit NOT NULL,
        [DataRecesso] datetime2 NULL,
        [GiustificatoMotivoDiRecesso] nvarchar(max) NULL,
        [RevisioneAccordo] bit NOT NULL,
        [CodiceComunicazioneMinisteroLavoro] nvarchar(max) NULL,
        [CREATION_DATE] datetime2 NOT NULL,
        [EDIT_TIME] datetime2 NOT NULL,
        [ParentId] uniqueidentifier NOT NULL,
        [ChildId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Accordi] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_Accordi_Codice] UNIQUE ([Codice])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240701174044_InitDB'
)
BEGIN
    CREATE TABLE [Valutazione] (
        [id] int NOT NULL IDENTITY,
        [AccordoId] uniqueidentifier NOT NULL,
        [NoteDipendente] nvarchar(max) NULL,
        [DataNoteDipendente] datetime2 NULL,
        [NoteDirigente] nvarchar(max) NULL,
        [DataNoteDirigente] datetime2 NULL,
        [isApprovata] bit NOT NULL,
        CONSTRAINT [PK_Valutazione] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240701174044_InitDB'
)
BEGIN
    CREATE TABLE [AttivitaAccordo] (
        [Id] int NOT NULL IDENTITY,
        [AccordoId] uniqueidentifier NOT NULL,
        [Index] int NOT NULL,
        [Attivita] nvarchar(max) NULL,
        [Risultati] nvarchar(max) NULL,
        [DenominazioneIndicatore] nvarchar(max) NULL,
        [TipologiaIndicatore] nvarchar(max) NULL,
        [OperatoreLogicoIndicatoreTesto] nvarchar(max) NULL,
        [TestoTarget] nvarchar(max) NULL,
        [TestoTargetRaggiunto] nvarchar(max) NULL,
        [OperatoreLogicoIndicatoreNumeroAssoluto] nvarchar(max) NULL,
        [NumeroAssolutoTarget] nvarchar(max) NULL,
        [NumeroAssolutoRaggiunto] nvarchar(max) NULL,
        [NumeroAssolutoDaTarget] nvarchar(max) NULL,
        [NumeroAssolutoATarget] nvarchar(max) NULL,
        [PercentualeIndicatoreDenominazioneNumeratore] nvarchar(max) NULL,
        [PercentualeIndicatoreDenominazioneDenominatore] nvarchar(max) NULL,
        [OperatoreLogicoIndicatorePercentuale] nvarchar(max) NULL,
        [PercentualeTarget] nvarchar(max) NULL,
        [PercentualeDenominatoreTargetRaggiunto] nvarchar(max) NULL,
        [PercentualeNumeratoreTargetRaggiunto] nvarchar(max) NULL,
        [PercentualeTargetRaggiunto] nvarchar(max) NULL,
        [PercentualeDaTarget] nvarchar(max) NULL,
        [PercentualeATarget] nvarchar(max) NULL,
        [OperatoreLogicoIndicatoreData] nvarchar(max) NULL,
        [DataTarget] datetime2 NULL,
        [DataTargetRaggiunto] datetime2 NULL,
        [DataDaTarget] datetime2 NULL,
        [DataATarget] datetime2 NULL,
        CONSTRAINT [PK_AttivitaAccordo] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AttivitaAccordo_Accordi_AccordoId] FOREIGN KEY ([AccordoId]) REFERENCES [Accordi] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240701174044_InitDB'
)
BEGIN
    CREATE TABLE [StoricoStato] (
        [Id] uniqueidentifier NOT NULL,
        [Stato] int NOT NULL,
        [Note] nvarchar(max) NULL,
        [Autore] nvarchar(max) NULL,
        [Timestamp] datetime2 NOT NULL,
        [AccordoId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_StoricoStato] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StoricoStato_Accordi_AccordoId] FOREIGN KEY ([AccordoId]) REFERENCES [Accordi] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240701174044_InitDB'
)
BEGIN
    CREATE TABLE [TransmissionStatus] (
        [AccordoId] uniqueidentifier NOT NULL,
        [WorkingDaysSentSuccessfully] bit NOT NULL,
        [LastWorkingDaysSentDate] datetime2 NULL,
        [WorkingDaysSendError] nvarchar(max) NULL,
        [WorkingActivitiesSentSuccessfully] bit NOT NULL,
        [LastWorkingActivitiesSentDate] datetime2 NULL,
        [WorkingActivitiesSendError] nvarchar(max) NULL,
        [NuovaComunicazioneMinisteroLavoroSentSuccessfully] bit NOT NULL,
        [NuovaComunicazioneMinisteroLavoroLastSentDate] datetime2 NULL,
        [NuovaComunicazioneMinisteroLavoroSendError] nvarchar(max) NULL,
        [RecessoComunicazioneMinisteroLavoroSentSuccessfully] bit NOT NULL,
        [RecessoComunicazioneMinisteroLavoroLastSentDate] datetime2 NULL,
        [RecessoComunicazioneMinisteroLavoroSendError] nvarchar(max) NULL,
        CONSTRAINT [PK_TransmissionStatus] PRIMARY KEY ([AccordoId]),
        CONSTRAINT [FK_TransmissionStatus_Accordi_AccordoId] FOREIGN KEY ([AccordoId]) REFERENCES [Accordi] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240701174044_InitDB'
)
BEGIN
    CREATE INDEX [IX_AttivitaAccordo_AccordoId] ON [AttivitaAccordo] ([AccordoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240701174044_InitDB'
)
BEGIN
    CREATE INDEX [IX_StoricoStato_AccordoId] ON [StoricoStato] ([AccordoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240701174044_InitDB'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240701174044_InitDB', N'8.0.6');
END;
GO

COMMIT;
GO

