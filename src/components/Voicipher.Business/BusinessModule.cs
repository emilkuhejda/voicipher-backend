using System.Reflection;
using Autofac;
using AutoMapper;
using Voicipher.Business.Channels;
using Voicipher.Business.Queries.ControlPanel;
using Voicipher.Business.Services;
using Voicipher.Business.StateMachine;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Channels;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.Interfaces.Queries.ControlPanel;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Interfaces.StateMachine;
using Module = Autofac.Module;

namespace Voicipher.Business
{
    public class BusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterServices(builder);
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly).Where(t => t.IsClosedTypeOf(typeof(ICommand<,>)))
                .AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(assembly).Where(t => t.IsClosedTypeOf(typeof(ICommand<>)))
                .AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(assembly).Where(t => t.IsClosedTypeOf(typeof(IQuery<,>)))
                .AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(assembly).Where(t => t.IsClosedTypeOf(typeof(IQuery<>)))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assembly).Where(t => t.IsAssignableTo<Profile>()).As<Profile>().AsSelf();

            builder.RegisterGeneric(typeof(GetInternalValueQuery<>)).As(typeof(IGetInternalValueQuery<>));

            builder.RegisterType<MessageCenterService>().As<IMessageCenterService>();
            builder.RegisterType<AudioService>().As<IAudioService>();
            builder.RegisterType<WavFileService>().As<IWavFileService>();
            builder.RegisterType<SpeechRecognitionService>().As<ISpeechRecognitionService>();
            builder.RegisterType<FileAccessService>().As<IFileAccessService>();
            builder.RegisterType<ZipFileService>().As<IZipFileService>();

            builder.RegisterType<ChunkStorage>().Keyed<IDiskStorage>(StorageLocation.Chunk);
            builder.RegisterType<AudioStorage>().Keyed<IDiskStorage>(StorageLocation.Audio);
            builder.RegisterType<BackupStorage>().Keyed<IDiskStorage>(StorageLocation.Backup);
            builder.RegisterType<BlobStorage>().As<IBlobStorage>();

            builder.RegisterType<JobStateMachine>().As<IJobStateMachine>();

            builder.RegisterType<MailProcessingChannel>().As<IMailProcessingChannel>().SingleInstance();
            builder.RegisterType<AudioFileProcessingChannel>().As<IAudioFileProcessingChannel>().SingleInstance();
        }
    }
}
