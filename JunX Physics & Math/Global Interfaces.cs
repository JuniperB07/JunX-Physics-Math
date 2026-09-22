using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JunX
{
    #region INITIALIZATIONS
    /// <summary>
    /// Defines a contract for a value type that supports static initialization and creation factory methods.
    /// </summary>
    /// <typeparam name="T">
    /// The structure type implementing this interface. Must be a value type (<see langword="struct"/>).
    /// </typeparam>
    /// <remarks>
    /// This interface uses C# 11 Static Abstract Members in Interfaces to enforce that implementing types 
    /// provide static creation patterns, enabling generic algorithms to construct instances of <typeparamref name="T"/> 
    /// without relying on reflection or instantiating factory objects.
    /// </remarks>
    public interface IInitializable<T>
    {
        /// <summary>
        /// Initializes and returns a default or starting instance of <typeparamref name="T"/>.
        /// </summary>
        /// <returns>
        /// A newly initialized instance of <typeparamref name="T"/> with default or starting state.
        /// </returns>
        static abstract T Initialize();
        /// <summary>
        /// Creates and returns a new instance of <typeparamref name="T"/> derived from or initialized with the provided arguments.
        /// </summary>
        /// <param name="Arg">
        /// The input value or instance of <typeparamref name="T"/> used to construct the new instance.
        /// </param>
        /// <returns>
        /// A new instance of <typeparamref name="T"/> created using the provided <paramref name="Arg"/>.
        /// </returns>
        static abstract T Create(T Arg);
    }
    /// <summary>
    /// Defines a contract for a value type that can be initialized or constructed from a specified input type.
    /// </summary>
    /// <typeparam name="T">
    /// The structure type implementing this interface. Must be a value type (<see langword="struct"/>).
    /// </typeparam>
    /// <typeparam name="T1">
    /// The type of the argument used to construct or initialize instances of <typeparamref name="T"/>.
    /// </typeparam>
    /// <remarks>
    /// This interface leverages static abstract members introduced in C# 11 to enforce factory methods 
    /// on value types, allowing generic code to instantiate <typeparamref name="T"/> directly from 
    /// an argument of type <typeparamref name="T1"/> without reflection overhead or runtime factory allocations.
    /// </remarks>
    public interface IInitializable<T, T1>
    {
        /// <summary>
        /// Creates and returns a new instance of <typeparamref name="T"/> using the provided input argument.
        /// </summary>
        /// <param name="Arg">
        /// The input argument of type <typeparamref name="T1"/> used to construct or populate the instance.
        /// </param>
        /// <returns>
        /// A new instance of <typeparamref name="T"/> initialized with the specified <paramref name="Arg"/>.
        /// </returns>
        static abstract T Create(T1 Arg);
    }
    /// <summary>
    /// Defines a contract for a value type that can be initialized or constructed from two specified input types.
    /// </summary>
    /// <typeparam name="T">
    /// The structure type implementing this interface. Must be a value type (<see langword="struct"/>).
    /// </typeparam>
    /// <typeparam name="T1">
    /// The type of the first argument used to construct or initialize instances of <typeparamref name="T"/>.
    /// </typeparam>
    /// <typeparam name="T2">
    /// The type of the second argument used to construct or initialize instances of <typeparamref name="T"/>.
    /// </typeparam>
    /// <remarks>
    /// This interface leverages static abstract members introduced in C# 11 to enforce multi-parameter factory methods 
    /// on value types, enabling generic algorithms to instantiate <typeparamref name="T"/> directly from input arguments 
    /// without reflection overhead or runtime factory allocations.
    /// </remarks>
    public interface IInitializable<T, T1, T2>
    {
        /// <summary>
        /// Creates and returns a new instance of <typeparamref name="T"/> using the provided input arguments.
        /// </summary>
        /// <param name="Arg1">
        /// The first input argument of type <typeparamref name="T1"/> used to construct the instance.
        /// </param>
        /// <param name="Arg2">
        /// The second input argument of type <typeparamref name="T2"/> used to construct the instance.
        /// </param>
        /// <returns>
        /// A new instance of <typeparamref name="T"/> initialized with the specified arguments.
        /// </returns>
        static abstract T Create(T1 Arg1, T2 Arg2);
    }
    /// <summary>
    /// Defines a contract for a value type that can be initialized or constructed from three specified input types.
    /// </summary>
    /// <typeparam name="T">
    /// The structure type implementing this interface. Must be a value type (<see langword="struct"/>).
    /// </typeparam>
    /// <typeparam name="T1">
    /// The type of the first argument used to construct or initialize instances of <typeparamref name="T"/>.
    /// </typeparam>
    /// <typeparam name="T2">
    /// The type of the second argument used to construct or initialize instances of <typeparamref name="T"/>.
    /// </typeparam>
    /// <typeparam name="T3">
    /// The type of the third argument used to construct or initialize instances of <typeparamref name="T"/>.
    /// </typeparam>
    /// <remarks>
    /// This interface leverages static abstract members introduced in C# 11 to enforce multi-parameter factory methods 
    /// on value types, enabling generic algorithms to instantiate <typeparamref name="T"/> directly from input arguments 
    /// without reflection overhead or runtime factory allocations.
    /// </remarks>
    public interface IInitializable<T, T1, T2, T3>
    {
        /// <summary>
        /// Creates and returns a new instance of <typeparamref name="T"/> using the provided input arguments.
        /// </summary>
        /// <param name="Arg1">
        /// The first input argument of type <typeparamref name="T1"/> used to construct the instance.
        /// </param>
        /// <param name="Arg2">
        /// The second input argument of type <typeparamref name="T2"/> used to construct the instance.
        /// </param>
        /// <param name="Arg3">
        /// The third input argument of type <typeparamref name="T3"/> used to construct the instance.
        /// </param>
        /// <returns>
        /// A new instance of <typeparamref name="T"/> initialized with the specified arguments.
        /// </returns>
        static abstract T Create(T1 Arg1, T2 Arg2, T3 Arg3);
    }
    /// <summary>
    /// Defines a contract for a value type that can be initialized or constructed from four specified input types.
    /// </summary>
    /// <typeparam name="T">
    /// The structure type implementing this interface. Must be a value type (<see langword="struct"/>).
    /// </typeparam>
    /// <typeparam name="T1">
    /// The type of the first argument used to construct or initialize instances of <typeparamref name="T"/>.
    /// </typeparam>
    /// <typeparam name="T2">
    /// The type of the second argument used to construct or initialize instances of <typeparamref name="T"/>.
    /// </typeparam>
    /// <typeparam name="T3">
    /// The type of the third argument used to construct or initialize instances of <typeparamref name="T"/>.
    /// </typeparam>
    /// <typeparam name="T4">
    /// The type of the fourth argument used to construct or initialize instances of <typeparamref name="T"/>.
    /// </typeparam>
    /// <remarks>
    /// This interface leverages static abstract members introduced in C# 11 to enforce multi-parameter factory methods 
    /// on value types, enabling generic algorithms to instantiate <typeparamref name="T"/> directly from input arguments 
    /// without reflection overhead or runtime factory allocations.
    /// </remarks>
    public interface IInitializable<T, T1, T2, T3, T4>
    {
        /// <summary>
        /// Creates and returns a new instance of <typeparamref name="T"/> using the provided input arguments.
        /// </summary>
        /// <param name="Arg1">
        /// The first input argument of type <typeparamref name="T1"/> used to construct the instance.
        /// </param>
        /// <param name="Arg2">
        /// The second input argument of type <typeparamref name="T2"/> used to construct the instance.
        /// </param>
        /// <param name="Arg3">
        /// The third input argument of type <typeparamref name="T3"/> used to construct the instance.
        /// </param>
        /// <param name="Arg4">
        /// The fourth input argument of type <typeparamref name="T4"/> used to construct the instance.
        /// </param>
        /// <returns>
        /// A new instance of <typeparamref name="T"/> initialized with the specified arguments.
        /// </returns>
        static abstract T Create(T1 Arg1, T2 Arg2, T3 Arg3, T4 Arg4);
    }
    #endregion

    #region NORMALIZATIONS
    public interface INormalized<En> where En : Enum
    {
        (double Magnitude, En Scale, int ScaleOrdinal) Normalized { get; }
        static abstract En BaseScale { get; }
    }
    public interface INormalizable<Str> where Str : struct
    {
        Str Normalize();
    }
    #endregion

    #region EXPONENTIABLES
    public interface IExponentiable<T>
    {
        T Squared();
        T Cubed();
        T Pow(int n);
    }
    public interface IExponentiable<T1, T2, T3>
    {
        T1 Squared();
        T2 Cubed();
        T3 Pow(int n);
    }
    #endregion

    #region DIMENSION ACCESSIBILITY
    public interface IDimensionAccessible
    {
        int Dimension { get; }
    }
    #endregion

    #region ROOTABILITY
    public interface ISquareRootable<T>
    {
        T Sqrt();
    }
    public interface ICubeRootable<T>
    {
        T CubeRt();
    }
    public interface IRootable<T>
    {
        T Root(int index);
    }
    #endregion

    #region SCALE MAPPABILITY
    public interface IScaleMappable<En> where En : Enum
    {
        static abstract Dictionary<En, double> Mapper { get; }
    }
    #endregion

    #region VALUE ACCESSIBILITY
    public interface IValueAccessible<En> where En : Enum
    {
        (double Magnitude, En Scale, int ScaleOrdinal) Original { get; }
        (double Magnitude, En Scale, int ScaleOrdinal) Converted { get; }
    }
    #endregion

    #region SCALE CONVERTABILITY
    public interface IScaleConvertible<Str, En>
        where Str : struct
        where En : Enum
    {
        abstract Str Convert(En toScale);
        double As(En scale);
    }
    #endregion

    #region DUPLICATABILITY & VALIDATABILITY
    public interface IDuplicatable<Str> where Str : struct
    {
        Str Duplicate();
    }
    public interface IValidatable
    {
        bool IsValid();
    }
    #endregion

    #region SCALE VALUE ACCESSIBILITY
    public interface IScaleValueAccessible<En1, En2>
        where En1 : Enum
        where En2 : Enum
    {
        public (En1 Scale1, En2 Scale2) ScaleValues { get; }
    }
    #endregion

    #region COMPOSITE UNIT
    public interface ICompositeUnit { }
    #endregion

    #region LINEAR UNITS
    public interface ILinearUnit<TSelf, TEnum> :
        IDimensionAccessible,
        IInitializable<TSelf>, IInitializable<TSelf, double>, IInitializable<TSelf, double, TEnum>,
        INormalized<TEnum>, INormalizable<TSelf>,
        IScaleConvertible<TSelf, TEnum>, IValueAccessible<TEnum>

        where TSelf : struct, ILinearUnit<TSelf, TEnum>
        where TEnum : Enum
    { }
    #endregion
}
