/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 *
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2017-6-10
 */
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.BusinessRules.JavaScript;
using OpenIZ.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using OpenIZ.Core.Interfaces;
using System.ComponentModel;
using OpenIZ.Core.Model;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Services;
using Jint.Runtime.Debugger;
using System.Threading;
using Jint.Native;
using Newtonsoft.Json;
using OpenIZ.Core.Diagnostics;
using System.Diagnostics.Tracing;
using OpenIZ.BusinessRules.JavaScript.JNI;
using Jint.Runtime.Interop;
using Newtonsoft.Json.Converters;
using System.Dynamic;
using OpenIZ.Core.Applets.ViewModel.Json;
using OpenIZ.Core.Services.Impl;

namespace OizDevTool.Debugger
{
    /// <summary>
    /// Business Rules debugger
    /// </summary>
    [Description("Business Rules Debugger")]
    public class BreDebugger : DebuggerBase
    {

        private bool m_blocPrint = false;
        private bool m_isStepRegistered = false;

        // Running thread
        private Thread m_runThread = null;

        // Loaded files
        private Dictionary<String, String> m_loadedFiles = new Dictionary<string, string>();

        /// <summary>
        /// Thread event
        /// </summary>
        private ManualResetEvent m_resetEvent = new ManualResetEvent(false);

        /// <summary>
        /// Step mode
        /// </summary>
        private StepMode? m_stepMode;

        /// <summary>
        /// Current debug information
        /// </summary>
        private DebugInformation m_currentDebug;

        /// <summary>
        /// Rule file
        /// </summary>
        private String m_loadFile = String.Empty;

        public class ConsoleTraceWriter : TraceWriter
        {
            /// <summary>
            /// Write
            /// </summary>
            public ConsoleTraceWriter(EventLevel filter, string initializationData) : base(filter, initializationData)
            {
            }

            protected override void WriteTrace(EventLevel level, string source, string format, params object[] args)
            {
                if (source == typeof(JsConsoleProvider).FullName)
                    Console.WriteLine(format, args);
            }

        }

        /// <summary>
        /// Service manager
        /// </summary>
        private class ServiceManager : IServiceManager
        {
            /// <summary>
            /// Get a service 
            /// </summary>
            public void AddServiceProvider(Type serviceType)
            {
                ApplicationContext.Current.AddServiceProvider(serviceType);
            }

            /// <summary>
            /// Get all services
            /// </summary>
            public IEnumerable<object> GetServices()
            {
                return ApplicationContext.Current.GetServices();
            }

            /// <summary>
            /// Remove service provider
            /// </summary>
            public void RemoveServiceProvider(Type serviceType)
            {
                ApplicationContext.Current.RemoveServiceProvider(serviceType);
            }
        }

        /// <summary>
        /// File system resolver
        /// </summary>
        private class FileSystemResolver : IDataReferenceResolver
        {
            public String RootDirectory { get; set; }

            public FileSystemResolver()
            {
                this.RootDirectory = Environment.CurrentDirectory;
            }

            /// <summary>
            /// Resolve specified reference
            /// </summary>
            public Stream Resolve(string reference)
            {
                reference = reference.Replace("~", this.RootDirectory);
                if (File.Exists(reference))
                    return File.OpenRead(reference);
                else
                {
                    Console.Error.WriteLine("ERR: {0}", reference);
                    return null;
                }
            }
        }

        /// <summary>
        /// BRE debugger
        /// </summary>
        /// <param name="sources"></param>
        public BreDebugger(StringCollection sources, String rootPath) : base(rootPath)
        {

            ApplicationContext.Current.AddServiceProvider(typeof(FileConfigurationService));


            ApplicationServiceContext.Current = ApplicationContext.Current;
            ApplicationContext.Current.AddServiceProvider(typeof(FileSystemResolver));
            ApplicationContext.Current.AddServiceProvider(typeof(ServiceManager));
            Tracer.AddWriter(new ConsoleTraceWriter(EventLevel.LogAlways, "dbg"), EventLevel.LogAlways);
            ApplicationContext.Current.Start();

            if (!String.IsNullOrEmpty(rootPath))
                ApplicationContext.Current.GetService<FileSystemResolver>().RootDirectory = rootPath;
            rootPath = ApplicationContext.Current.GetService<FileSystemResolver>().RootDirectory;

            // Load debug targets
            Console.WriteLine("Loading debuggees...");
            if (sources != null)
                foreach (var rf in sources)
                {
                    var f = rf.Replace("~", rootPath);
                    if (!File.Exists(f))
                        Console.Error.WriteLine("Can't find file {0}", f);
                    else
                        this.Execute(f);
                }
            JavascriptBusinessRulesEngine.Current.Engine.Step += JreStep;

        }

        /// <summary>
        /// Register for step
        /// </summary>
        public void StepRegister(bool state)
        {

            this.m_isStepRegistered = state;
        }

        /// <summary>
        /// Step is called
        /// </summary>
        private Jint.Runtime.Debugger.StepMode JreStep(object sender, Jint.Runtime.Debugger.DebugInformation e)
        {
            if (this.m_stepMode.HasValue && (this.m_stepMode == StepMode.Over || this.m_stepMode == StepMode.Into) || this.m_isStepRegistered || this.Breakpoints.Contains(e.CurrentStatement.Location.Start.Line))
            {
                var col = Console.ForegroundColor;
                Console.ForegroundColor = this.GetResponseColor();
                this.m_prompt = $"{e.CurrentStatement.LabelSet ?? JavascriptBusinessRulesEngine.Current.ExecutingFile ?? this.m_loadFile} @ {e.CurrentStatement.Location.Start.Line} (step) >";
                this.m_currentDebug = e;
                int l = Console.CursorLeft;
                Console.CursorLeft = 0;
                Console.Write(new string(' ', l));
                Console.CursorLeft = 0;
                if (this.m_blocPrint)
                    this.PrintBlock();
                else
                    this.PrintLoc();
                Console.ForegroundColor = col;
                this.Prompt();
                this.m_resetEvent.WaitOne();
                this.m_resetEvent.Reset();
                this.m_currentDebug = null;
                return this.m_stepMode ?? StepMode.Into;
            }
            else
                return StepMode.Into;
        }

        [Command("ob", "Sets the break output to block mode")]
        public void OutputBlock() => this.m_blocPrint = true;

        [Command("ol", "Sets the break output to line mode")]
        public void OutputLine() => this.m_blocPrint = false;

        /// <summary>
        /// Loads a script to be debugged
        /// </summary>
        [Command("o", "Open a script file to be debugged")]
        public void LoadScript(String file)
        {
            if (!Path.IsPathRooted(file))
                file = Path.Combine(this.m_workingDirectory, file);
            if (!File.Exists(file))
                throw new FileNotFoundException(file);
            if (!String.IsNullOrEmpty(this.m_loadFile))
                Console.WriteLine("WARN: The file you're attempting to load is already loaded but not executed. Reloading.");

            String key = Path.GetFileName(file);
            if (this.m_loadedFiles.ContainsKey(key))
                throw new InvalidOperationException($"File {key} already loaded and executed (or executing)");
            this.m_loadedFiles.Add(key, file);
            this.m_loadFile = file;
            this.m_prompt = key + " (idle) >";

        }

        /// <summary>
        /// Loads a script to be debugged
        /// </summary>
        [Command("x", "Executes the loaded file")]
        public void Execute()
        {
            // Exec = continue execution
            if (this.m_currentDebug != null)
            {
                this.StepRegister(false);
                this.m_resetEvent.Set();
                return;
            }

            if (String.IsNullOrEmpty(this.m_loadFile))
                throw new InvalidOperationException("No file in buffer");
            else if (this.m_runThread != null)
                throw new InvalidOperationException("Script is already running...");
            this.m_prompt = Path.GetFileName(this.m_loadFile) + " (run) >";

            this.m_runThread = new Thread(() =>
            {
                using (var sr = File.OpenText(this.m_loadFile))
                    JavascriptBusinessRulesEngine.Current.AddRules(Path.GetFileName(this.m_loadFile), sr);
                this.m_prompt = Path.GetFileName(this.m_loadFile) + " (idle) >";
                this.m_loadFile = Path.GetFileName(this.m_loadFile);
                Console.WriteLine("\r\nExecution Finished");
                this.Prompt();

            });
            this.m_runThread.Start();
        }

        /// <summary>
        /// Loads a script to be debugged
        /// </summary>
        [Command("x", "Loads and executes the specified file")]
        public void Execute(String file)
        {
            this.LoadScript(file);
            this.Execute();
        }

        /// <summary>
        /// Set a breakpoint
        /// </summary>
        [Command("gn", "Go until next breakpoint")]
        public void GoNext()
        {
            this.StepRegister(false);

            if (this.m_currentDebug != null)
            {

                this.m_stepMode = null;
                this.m_resetEvent.Set();

            }
            else
                throw new InvalidOperationException("Step-in can only be done when loaded but not executing");
        }

        /// <summary>
        /// Set a breakpoint
        /// </summary>
        [Command("gi", "Steps into the current line number")]
        public void GoIn()
        {
            this.StepRegister(true);

            if (this.m_currentDebug != null)
            {
                this.m_stepMode = StepMode.Into;
                this.m_resetEvent.Set();
            }
            else if (!String.IsNullOrEmpty(this.m_loadFile))
            {
                this.Execute();
            }
            else
                throw new InvalidOperationException("Step-in can only be done when loaded but not executing");
        }

        /// <summary>
        /// Set a breakpoint
        /// </summary>
        [Command("go", "Steps over the current line number")]
        public void GoOver()
        {
            if (this.m_currentDebug != null)
            {
                this.m_stepMode = StepMode.Over;
                this.m_resetEvent.Set();
            }
            else if (!String.IsNullOrEmpty(this.m_loadFile))
            {
                this.StepRegister(false);

                this.Execute();
            }
            else
                throw new InvalidOperationException("Step-over can only be done when loaded but not executing");
        }

        /// <summary>
        /// Set a breakpoint
        /// </summary>
        [Command("gu", "Steps out of the current scope ")]
        public void GoOut()
        {
            if (this.m_currentDebug != null)
            {
                this.m_stepMode = StepMode.Out;
                this.m_resetEvent.Set();
            }
            else if (!String.IsNullOrEmpty(this.m_loadFile))
            {
                this.StepRegister(false);

                this.Execute();
            }
            else
                throw new InvalidOperationException("Step-over can only be done when loaded but not executing");
        }

        /// <summary>
        /// Dump locals
        /// </summary>
        [Command("dl", "Dumps local variables to console")]
        public void DumpLocals()
        {
            this.DumpLocals(null);
        }

        /// <summary>
        /// Dump locals
        /// </summary>
        [Command("dl", "Dumps specified local variable to console")]
        public void DumpLocals(String id)
        {
            this.DumpLocals(id, null);
        }

        /// <summary>
        /// Dump locals
        /// </summary>
        [Command("dl", "Dumps specified local variable path to console")]
        public void DumpLocals(String id, String path)
        {
            this.ThrowIfNotDebugging();
            // Locals?
            if (id == null)
                this.DumpObject(this.m_currentDebug.Locals, path);
            else
            {
                var kobj = this.m_currentDebug.Locals[id];
                try
                {
                    kobj = JavascriptBusinessRulesEngine.Current.Engine.GetValue(kobj);
                    if (kobj.Is<Jint.Runtime.Interop.ObjectWrapper>())
                        this.DumpObject((kobj.AsObject() as ObjectWrapper).Target, path);
                    else
                        this.DumpObject(kobj?.AsObject()?.GetOwnProperties(), path);

                }
                catch
                {
                    this.DumpObject(kobj?.AsObject()?.GetOwnProperties(), path);
                }
            }
        }

        /// <summary>
        /// Dump scope as json
        /// </summary>
        /// <param name="path"></param>
        [Command("dlv", "Dumps local as ViewModel JSON to screen")]
        public void DumpLocalsJson(String id)
        {
            this.DumpLocalsJson(id, null);
        }

        /// <summary>
        /// Dump scope
        /// </summary>
        /// <param name="path"></param>
        [Command("dlv", "Dumps local as ViewModel JSON to screen")]
        public void DumpLocalsJson(String id, String path)
        {

            this.ThrowIfNotDebugging();
            // Locals?
            if (id == null)
                this.DumpObject(this.m_currentDebug.Locals, path);
            else
            {
                var kobj = this.m_currentDebug.Locals[id];
                try
                {
                    kobj = JavascriptBusinessRulesEngine.Current.Engine.GetValue(kobj);
                    if (kobj.Is<Jint.Runtime.Interop.ObjectWrapper>())
                    {
                        Object obj = new Dictionary<String, Object>((kobj.AsObject() as ObjectWrapper).Target as ExpandoObject);
                        obj = JavascriptBusinessRulesEngine.Current.Bridge.ToModel(this.GetScopeObject(obj, path));
                        JsonViewModelSerializer xsz = new JsonViewModelSerializer();
                        using (var jv = new JsonTextWriter(Console.Out) { Formatting = Newtonsoft.Json.Formatting.Indented })
                        {
                            xsz.Serialize(jv, obj as IdentifiedData);
                        }
                        Console.WriteLine();
                    }
                    else
                        this.DumpObject(kobj?.AsArray() ?? kobj?.AsObject(), path);

                }
                catch
                {
                    this.DumpObject(kobj?.AsObject()?.GetOwnProperties(), path);
                }
            }

            

        }
        /// <summary>
        /// Dump locals
        /// </summary>
        [Command("dg", "Dumps global variables to console")]
        public void DumpGlobals()
        {
            this.DumpGlobals(null);
        }

        /// <summary>
        /// Dump locals
        /// </summary>
        [Command("dg", "Dumps specified global variable to console")]
        public void DumpGlobals(String id)
        {
            this.DumpGlobals(id, null);
        }

        /// <summary>
        /// Dump locals
        /// </summary>
        [Command("dg", "Dumps specified global variable path to console")]
        public void DumpGlobals(String id, String path)
        {
            this.ThrowIfNotDebugging();
            // Locals?
            if (id == null)
                this.DumpObject(this.m_currentDebug.Globals, path);
            else
            {
                var kobj = this.m_currentDebug.Globals[id];
                try
                {
                    kobj = JavascriptBusinessRulesEngine.Current.Engine.GetValue(kobj);
                    if (kobj.Is<Jint.Runtime.Interop.ObjectWrapper>())
                        this.DumpObject((kobj.AsObject() as ObjectWrapper).Target, path);
                    else
                        this.DumpObject(kobj?.AsObject()?.GetOwnProperties(), path);

                }
                catch
                {
                    this.DumpObject(kobj?.AsObject()?.GetOwnProperties(), path);
                }
            }

        }

        /// <summary>
        /// Dump locals
        /// </summary>
        [Command("cs", "Shows the current callstack")]
        public void CallStack()
        {
            this.ThrowIfNotDebugging();
            int i = 0;
            foreach (var itm in this.m_currentDebug.CallStack)
                Console.WriteLine("{0}:{1}", i++, itm);
        }

        /// <summary>
        /// Check in debug mode
        /// </summary>
        private void ThrowIfNotDebugging()
        {
            if (this.m_currentDebug == null)
                throw new InvalidOperationException("Not in break / debug mode");
        }

        /// <summary>
        /// Print line of code
        /// </summary>
        [Command("pl", "Prints the current line of code")]
        public void PrintLoc()
        {
            this.ThrowIfNotDebugging();
            this.PrintFile(this.m_currentDebug.CurrentStatement.Location.Start.Line.ToString(), this.m_currentDebug.CurrentStatement.Location.Start.Line.ToString());
        }

        /// <summary>
        /// Print block of code
        /// </summary>
        [Command("pb", "Prints the current block of code")]
        public void PrintBlock()
        {
            this.ThrowIfNotDebugging();
            this.PrintFile((this.m_currentDebug.CurrentStatement.Location.Start.Line - 5).ToString(), (this.m_currentDebug.CurrentStatement.Location.End.Line + 5).ToString());

        }

        /// <summary>
        /// Print block of code
        /// </summary>
        [Command("pf", "Prints the current file in memory")]
        public void PrintFile()
        {
            this.PrintFile(null, null);
        }

        /// <summary>
        /// Print file
        /// </summary>
        [Command("pf", "Prints the current file in memory from the start-end lines")]
        public void PrintFile(string start, string end)
        {
            String fileName = null;
            if (this.m_currentDebug != null)
            {
                if (!this.m_loadedFiles.TryGetValue(this.m_currentDebug.CurrentStatement.LabelSet ?? JavascriptBusinessRulesEngine.Current.ExecutingFile ?? this.m_loadFile, out fileName))
                    throw new InvalidOperationException($"Source for {this.m_currentDebug.CurrentStatement.LabelSet} not found");
            }
            else if (!this.m_loadedFiles.TryGetValue(Path.GetFileName(this.m_loadFile), out fileName))
                throw new InvalidOperationException($"Source for '{this.m_loadFile}' not found");

            int ln = 1,
                startInt = String.IsNullOrEmpty(start) ? 0 : Int32.Parse(start),
                endInt = String.IsNullOrEmpty(end) ? Int32.MaxValue : Int32.Parse(end);

            using (var sr = File.OpenText(fileName))
            {
                while (!sr.EndOfStream)
                {
                    if (ln >= startInt && ln <= endInt)
                        Console.WriteLine($"[{ln}]{(this.m_currentDebug?.CurrentStatement.Location.Start.Line <= ln && this.m_currentDebug?.CurrentStatement.Location.End.Line >= ln ? "++>" : this.Breakpoints.Contains(ln) ? "***" : "   ")}{sr.ReadLine()}");
                    else if (ln >= endInt) break;
                    else
                        sr.ReadLine();
                    ln++;
                }
                if (sr.EndOfStream)
                    Console.WriteLine("<<EOF>>");

            }
        }

        /// <summary>
        /// Executes a validator
        /// </summary>
        [Command("go.validate", "Executes the validator for the current scope object")]
        public void ExecuteValidator()
        {
            this.ExecuteValidator(false, this.m_scopeObject?.GetType());
        }

        /// <summary>
        /// Executes a validator
        /// </summary>
        [Command("gi.validate", "Steps into the validator for the current scope object casting it before running")]
        public void ExecuteStepValidator(string cast)
        {
            var t = Type.GetType(cast);
            if (t == null)
                t = new OpenIZ.Core.Model.Serialization.ModelSerializationBinder().BindToType(typeof(IdentifiedData).Assembly.FullName, cast);
            if (t == null)
            {
                throw new ArgumentOutOfRangeException(nameof(cast));
            }
            this.ExecuteValidator(true, t);
        }


        /// <summary>
        /// Executes a validator
        /// </summary>
        [Command("go.validate", "Executes the validator for the current scope object casting it before running")]
        public void ExecuteValidator(string cast)
        {
            var t = Type.GetType(cast);
            if (t == null)
                t = new OpenIZ.Core.Model.Serialization.ModelSerializationBinder().BindToType(typeof(IdentifiedData).Assembly.FullName, cast);
            if (t == null)
            {
                throw new ArgumentOutOfRangeException(nameof(cast));
            }
            this.ExecuteValidator(false, t);
        }

        /// <summary>
        /// Executes a validator
        /// </summary>
        [Command("gi.validate", "Steps into the validator for the current scope object")]
        public void ExecuteStepValidator()
        {
            this.ExecuteValidator(true, this.m_scopeObject?.GetType());
        }

        /// <summary>
        /// Execute a validator
        /// </summary>
        private void ExecuteValidator(bool stepIn, Type asType)
        {
            this.StepRegister(stepIn);

            if (this.m_scopeObject == null)
                throw new InvalidOperationException("Cannot validate a null scope object");
            else if (this.m_currentDebug != null)
                throw new InvalidOperationException("Cannot execute validator at this time");

            if (asType == null)
                asType = this.m_scopeObject.GetType();
            var rdb = typeof(IBusinessRulesService<>).MakeGenericType(asType);
            var rds = ApplicationContext.Current.GetService(rdb);
            if (rds == null)
                throw new InvalidOperationException($"Cannot find business rule registered for {this.m_scopeObject.GetType().Name}");
            else
            {
                this.m_prompt = Path.GetFileName(this.m_loadFile) + " (run) >";

                this.m_runThread = new Thread(() =>
                {
                    try
                    {
                        var mi = rdb.GetMethod("Validate");
                        foreach (var itm in mi.Invoke(rds, new object[] { this.m_scopeObject }) as List<DetectedIssue>)
                        {
                            Console.WriteLine("{0}\t{1}", itm.Priority, itm.Text);
                        }
                        this.m_prompt = Path.GetFileName(this.m_loadFile) + " (idle) >";
                        Console.WriteLine("\r\nExecution Complete");
                        this.Prompt();

                    }
                    catch (Exception e)
                    {
                        this.PrintStack(e);
                        this.Prompt();

                    }

                });
                this.m_runThread.Start();
            }
        }
        /// <summary>
        /// Executes a validator
        /// </summary>
        [Command("go.rule", "Executes the validator for the current scope object")]
        public void ExecuteRule(String @event)
        {
            this.ExecuteRule(@event, false, this.m_scopeObject?.GetType());
        }

        /// <summary>
        /// Executes a validator
        /// </summary>
        [Command("gi.rule", "Steps into the validator for the current scope object casting it before running")]
        public void ExecuteRule(String @event, string cast)
        {
            var t = Type.GetType(cast);
            if (t == null)
                t = new OpenIZ.Core.Model.Serialization.ModelSerializationBinder().BindToType(typeof(IdentifiedData).Assembly.FullName, cast);
            if (t == null)
            {
                throw new ArgumentOutOfRangeException(nameof(cast));
            }
            this.ExecuteRule(@event, true, t);
        }


        /// <summary>
        /// Executes a validator
        /// </summary>
        [Command("go.rule", "Executes the validator for the current scope object casting it before running")]
        public void ExecuteStepRule(String @event, string cast)
        {
            var t = Type.GetType(cast);
            if (t == null)
                t = new OpenIZ.Core.Model.Serialization.ModelSerializationBinder().BindToType(typeof(IdentifiedData).Assembly.FullName, cast);
            if (t == null)
            {
                throw new ArgumentOutOfRangeException(nameof(cast));
            }
            this.ExecuteRule(@event, false, t);
        }

        /// <summary>
        /// Executes a validator
        /// </summary>
        [Command("gi.rule", "Steps into the validator for the current scope object")]
        public void ExecuteStepRule(String @event)
        {
            this.ExecuteRule(@event, true, this.m_scopeObject?.GetType());
        }

        /// <summary>
        /// Execute a validator
        /// </summary>
        private void ExecuteRule(String @event, bool stepIn, Type asType)
        {
            this.StepRegister(stepIn);

            if (this.m_scopeObject == null)
                throw new InvalidOperationException("Cannot validate a null scope object");
            else if (this.m_currentDebug != null)
                throw new InvalidOperationException("Cannot execute validator at this time");

            if (asType == null)
                asType = this.m_scopeObject.GetType();
            var rdb = typeof(IBusinessRulesService<>).MakeGenericType(asType);
            var rds = ApplicationContext.Current.GetService(rdb);
            if (rds == null)
                throw new InvalidOperationException($"Cannot find business rule registered for {this.m_scopeObject.GetType().Name}");
            else
            {
                this.m_prompt = Path.GetFileName(this.m_loadFile) + " (run) >";

                this.m_runThread = new Thread(() =>
                {
                    try
                    {
                        var mi = rdb.GetMethod(@event);
                        this.m_scopeObject = mi.Invoke(rds, new object[] { this.m_scopeObject });
                        this.m_prompt = Path.GetFileName(this.m_loadFile) + " (idle) >";
                        Console.WriteLine("\r\nExecution Complete, result set to scope");
                        this.Prompt();
                    }
                    catch (Exception e)
                    {
                        this.PrintStack(e);
                        this.Prompt();

                    }
                });
                this.m_runThread.Start();
            }
        }

        /// <summary>
        /// Exit the specified context
        /// </summary>
        public override void Exit()
        {
            this.m_runThread?.Abort();
            base.Exit();
        }
    }

}
