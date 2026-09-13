namespace PdfEditor.Core.Errors;

public enum PdfErrorKind
{
    FileNotFound,
    PermissionDenied,
    CorruptDocument,
    UnsupportedDocument,
    PasswordRequired,
    IncorrectPassword,
    InvalidPage,
    RenderingFailed,
    SaveFailed,
    DiskFull,
    FileChangedExternally,
    EngineFailure,
    OutOfMemory
}
