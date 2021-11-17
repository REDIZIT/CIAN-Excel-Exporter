using Zenject;

namespace InApp.DI
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<Pathes>().AsSingle();
            Container.Bind<Worker>().AsSingle();
        }
    }
}