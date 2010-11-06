﻿namespace IronJS

open System
open IronJS

module Hosting =

  let createEnvironment () =
    let x = IjsEnv()

    let objectMethods = {
      GetProperty = Api.Object.Property.Delegates.get
      HasProperty = Api.Object.Property.Delegates.has
      DeleteProperty = Api.Object.Property.Delegates.delete
      PutBoxProperty = Api.Object.Property.Delegates.putBox
      PutRefProperty = Api.Object.Property.Delegates.putRef
      PutValProperty = Api.Object.Property.Delegates.putVal

      GetIndex = Api.Object.Index.Delegates.get
      HasIndex = Api.Object.Index.Delegates.has
      DeleteIndex = Api.Object.Index.Delegates.delete
      PutBoxIndex = Api.Object.Index.Delegates.putBox
      PutRefIndex = Api.Object.Index.Delegates.putRef
      PutValIndex = Api.Object.Index.Delegates.putVal

      Default = Api.Object.defaultValue'
    }

    x.Methods <- {
      Object = objectMethods
      Array = 
        {objectMethods with
          PutBoxProperty = Api.Array.Property.Delegates.putBox
          PutRefProperty = Api.Array.Property.Delegates.putRef
          PutValProperty = Api.Array.Property.Delegates.putVal
        }

      Arguments = 
        {objectMethods with
          GetIndex = Api.Arguments.Index.Delegates.get
          HasIndex = Api.Arguments.Index.Delegates.has
          DeleteIndex = Api.Arguments.Index.Delegates.delete
          PutBoxIndex = Api.Arguments.Index.Delegates.putBox
          PutValIndex = Api.Arguments.Index.Delegates.putVal
          PutRefIndex = Api.Arguments.Index.Delegates.putRef
        }
    }

    x.FunctionMethods <- {
      HasInstance = new HasInstance(fun l r -> Api.Function.hasInstance(l, r))
    }

    let baseMap = PropertyMap(x)
    x.Maps <- {
      Base = baseMap
      Array = Api.PropertyMap.getSubMap baseMap "length"
      Function = Api.PropertyMap.buildSubMap baseMap ["length"; "prototype"]
      Prototype = Api.PropertyMap.getSubMap baseMap "constructor"
      String = Api.PropertyMap.getSubMap baseMap "length"
      Number = baseMap
      Boolean = baseMap
      Regexp = 
        Api.PropertyMap.buildSubMap baseMap [
          "source"; "global"; "ignoreCase"; "multiline"; "lastIndex"]
    }

    let objectPrototype = Native.Object.createPrototype x
    x.Prototypes <- Prototypes.Empty
    x.Prototypes <- {
      Object = objectPrototype
      Function = Native.Function.createPrototype x objectPrototype
      Array = Native.Array.createPrototype x objectPrototype
      String = Native.String.createPrototype x objectPrototype
      Number = Native.Number.createPrototype x objectPrototype
      Boolean = Native.Boolean.createPrototype x objectPrototype
      Date = null
      RegExp = null
      Error = Native.Error.createPrototype x objectPrototype

      EvalError = null
      RangeError = null
      ReferenceError = null
      SyntaxError = null
      TypeError = null
      URIError = null
    }
    
    x.Constructors <- Constructors.Empty

    Native.Global.setup x
    Native.Math.setup x
    Native.Object.setupPrototype x
    Native.Object.setupConstructor x
    Native.Function.setupConstructor x
    Native.Function.setupPrototype x
    Native.Array.setupConstructor x
    Native.Array.setupPrototype x
    Native.Error.setupConstructor x
    Native.Error.setupPrototype x

    x

  type Context(env:IjsEnv) =
    
    let globalFunc = new IronJS.Function(env)

    member x.Environment = env
    member x.GlobalFunc = globalFunc

    member x.CompileFile fileName =
      let tree = Parsers.Ecma3.parseGlobalFile env fileName
      let analyzed = Ast.Analyzers.applyDefault tree None
      Debug.printString (sprintf "%A" analyzed)

      Compiler.Core.compileAsGlobal env analyzed

    member x.CompileSource source =
      let tree = Parsers.Ecma3.parseGlobalSource env source
      let analyzed = Ast.Analyzers.applyDefault tree None
      Debug.printString (sprintf "%A" analyzed)

      Compiler.Core.compileAsGlobal env analyzed

    member x.InvokeCompiled (compiled:Delegate) =
      let result = compiled.DynamicInvoke(globalFunc, env.Globals)
      Utils.unboxObj result

    member x.ExecuteFile fileName = x.InvokeCompiled (x.CompileFile fileName)
    member x.ExecuteFileT<'a> fileName = x.ExecuteFile fileName :?> 'a
    member x.Execute source = x.InvokeCompiled (x.CompileSource source)
    member x.ExecuteT<'a> source = x.Execute source :?> 'a
    
    member x.EvalInFunc source = 
      x.Execute (sprintf "(function(){ %s })();" source)

    member x.EvalInFuncT<'a> source = 
      x.EvalInFunc source :?> 'a

    member x.PutGlobal (name, value:obj) =
      env.Globals.Methods.PutBoxProperty.Invoke(env.Globals, name, Utils.box value)

    member x.GetGlobal name =
      env.Globals.Methods.GetProperty.Invoke(env.Globals, name)

    member x.GetGlobalT<'a> name =
      let value = env.Globals.Methods.GetProperty.Invoke(env.Globals, name) 
      value |> Utils.unboxT<'a> 

    static member Create () =
      new Context(createEnvironment())