# NightlyCode.Modules

Simple module management which allows definition of modules with dependencies.
Can be used for simple plugin logic.

## Module Manager

Everything in module management is related to the ModuleManager. 

### Creation

To get started just create a simple ModuleManager with the following line.

```
ModuleManager<ModuleInformation> modulemanager=new ModuleManager<ModuleInformation>();
```

### Adding Modules

You have to define a class which implements IModule in some way.

```
public class MyModule : IModule {}
```

Then you can add this to the ModuleManager by using the following.

```
modulemanager.AddModule(new MyModule());
```

### Starting the ModuleManager

When you are done adding your modules you have to start the module manager. This will automatically start the modules in the correct order considering their dependencies and call **IRunnableModule.Start** if the module implements **IRunnableModule**.

```
modulemanager.Start();
```

### Stopping the ModuleManager

When your program is done or doesn't need the ModuleManager anymore you should stop the ModuleManager to allow for cleanup logic on modules.

```
modulemanager.Stop();
```

### Start/Stop modules manually

You can also start or stop specific modules while the ModuleManager is running. This will start/stop all modules which depend on the module you specified.

```
modulemanager.StartModule<MyModule>();
```

or

```
modulemanager.StopModule<MyModule>();
```