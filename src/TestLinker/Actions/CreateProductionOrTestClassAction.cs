// Copyright 2016, 2015, 2014 Matthias Koch
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.UI.ActionsRevised;

namespace TestLinker.Actions
{
  [Action ("Create_ProductionOrTestClass", "Create production/test class", Id = 9855)]
  public class CreateProductionOrTestClassAction : IActionWithExecuteRequirement, IExecutableAction
  {
    public IActionRequirement GetRequirement (IDataContext dataContext)
    {
      throw new System.NotImplementedException();
    }

    public bool Update (IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
    {
      throw new System.NotImplementedException();
    }

    public void Execute (IDataContext context, DelegateExecute nextExecute)
    {
      // Get type from namespace

      // Get type with linked type of priority 1

      // Create stub from template
      throw new System.NotImplementedException();
    }
  }
}