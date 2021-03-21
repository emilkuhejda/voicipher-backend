// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.Models.Administrator.PasswordHash")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.Models.Administrator.PasswordSalt")]
[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.Models.AudioFile.TranscribeItems")]
[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.OutputModels.Audio.TranscribeItemOutputModel.Alternatives")]
[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.OutputModels.Audio.RecognitionAlternativeOutputModel.Words")]
[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.Models.InformationMessage.LanguageVersions")]
[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.OutputModels.InformationMessageOutputModel.LanguageVersions")]
[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.Models.RecognizedAudioSample.SpeechResults")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.Transcription.SubscriptionProducts.All")]
[assembly: SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.Settings.AppSettings.GoogleApiAuthUri")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "By design.", Scope = "member", Target = "~P:Voicipher.Domain.Settings.SpeechCredentials.Project_id")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "By design.", Scope = "member", Target = "~P:Voicipher.Domain.Settings.SpeechCredentials.Private_key_id")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "By design.", Scope = "member", Target = "~P:Voicipher.Domain.Settings.SpeechCredentials.Private_key")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "By design.", Scope = "member", Target = "~P:Voicipher.Domain.Settings.SpeechCredentials.Client_email")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "By design.", Scope = "member", Target = "~P:Voicipher.Domain.Settings.SpeechCredentials.Client_id")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "By design.", Scope = "member", Target = "~P:Voicipher.Domain.Settings.SpeechCredentials.Auth_uri")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "By design.", Scope = "member", Target = "~P:Voicipher.Domain.Settings.SpeechCredentials.Token_uri")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "By design.", Scope = "member", Target = "~P:Voicipher.Domain.Settings.SpeechCredentials.Auth_provider_x509_cert_url")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "By design.", Scope = "member", Target = "~P:Voicipher.Domain.Settings.SpeechCredentials.Client_x509_cert_url")]
[assembly: SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "By design.", Scope = "member", Target = "~P:Voicipher.Domain.Settings.SpeechCredentials.Auth_uri")]
[assembly: SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "By design.", Scope = "member", Target = "~P:Voicipher.Domain.Settings.SpeechCredentials.Token_uri")]
[assembly: SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "By design.", Scope = "member", Target = "~P:Voicipher.Domain.Settings.SpeechCredentials.Auth_provider_x509_cert_url")]
[assembly: SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "By design.", Scope = "member", Target = "~P:Voicipher.Domain.Settings.SpeechCredentials.Client_x509_cert_url")]
[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.Notifications.NotificationContent.CustomData")]
[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.Payloads.Notifications.InformationMessagePayload.LanguageVersions")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.Settings.AppSettings.AllowedHosts")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.Interfaces.StateMachine.IMachineState.TranscribedAudioFiles")] 
[assembly: SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.Settings.AppSettings.ApiUrl")] 
[assembly: SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.Payloads.LastUpdatesOutputModel.ApiUrl")]
[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "It is appropriate.", Scope = "member", Target = "~P:Voicipher.Domain.Models.BillingPurchase.PurchaseStateTransactions")]
