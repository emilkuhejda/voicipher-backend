// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.Models.Administrator.PasswordHash")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.Models.Administrator.PasswordSalt")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.Payloads.GenerateTokenPayload.PasswordHash")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.Payloads.GenerateTokenPayload.PasswordSalt")]
[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.Models.AudioFile.TranscribeItems")]
[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.OutputModels.Audio.TranscribeItemOutputModel.Alternatives")]
[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.OutputModels.Audio.RecognitionAlternativeOutputModel.Words")]
[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.Models.InformationMessage.LanguageVersions")]
[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.OutputModels.InformationMessageOutputModel.LanguageVersions")]
