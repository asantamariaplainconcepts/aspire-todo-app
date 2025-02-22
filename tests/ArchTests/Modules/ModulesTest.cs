﻿using Notifications;

namespace ArchTests.Modules;

public class ModulesTest
{
    [Fact]
    public void TodosModule_DoesNotHave_Dependency_On_Other_Modules()
    {
        var otherModules = new[]
        {
            typeof(NotificationsModule).Namespace,
        };

        var result = AppTypes()
            .ShouldNot()
            .HaveDependencyOnAll(otherModules)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}