CREATE TABLE [dbo].[lampotilaTaulu] (
    [lampotila1] FLOAT (53) NULL,
    [lampotila2] FLOAT (53) NULL,
    [valoisuus]  FLOAT (53) NULL,
    [humidity]   FLOAT (53) NULL,
    [tempC]      FLOAT (53) NULL,
    [aika]       DATETIME   NULL,
    [id]         BIGINT     NULL,
    [rowId]      BIGINT     IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lampotilaTaulu] PRIMARY KEY CLUSTERED ([rowId] ASC)
);

