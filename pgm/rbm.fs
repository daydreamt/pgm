namespace pgm

module rbm =
    open System
    open System.Runtime.InteropServices
    open Microsoft.FSharp.NativeInterop
    open Microsoft.FSharp.Math
    
    type rbm_ = 
        val nv : int
        val nh : int
        val visible: Array
        val hidden: Array
        val weights: Array

    let get_number_of_weights nv nh =
        (nv * nh) + nv + nh;

    [<System.Runtime.InteropServices.DllImport("..\..\..\lib\pgm_cpp", EntryPoint="sigmoid", CallingConvention=CallingConvention.Cdecl)>]
    extern double sigmoid(double a);

    [<System.Runtime.InteropServices.DllImport("C:\Users\dd\Dropbox\pgm\pgm_cpp\pgm_cpp\Debug\pgm_cpp", EntryPoint="mk_rbm", CallingConvention=CallingConvention.Cdecl)>]
    //[<System.Runtime.InteropServices.DllImport("..\..\..\lib\pgm_cpp", EntryPoint="mk_rbm", CallingConvention=CallingConvention.Cdecl)>]
    extern void mk_rbm (
        uint32 nv,
        uint32 nh,
        [<Out;>]
        int[] visible,
        [<Out;>]
        int[] hidden,
        [<Out;>]
        double[] weights);


    let rbm_constructed (nv:uint32) (nh:uint32) = 

        let mutable visible = Array.zeroCreate<int> (int nv)
        let mutable hidden = Array.zeroCreate<int> (int nh)
        let mutable weights = Array.zeroCreate<double> (get_number_of_weights (int nv) (int nh))

        mk_rbm(nv, nh, visible, hidden, weights)

        (visible, hidden, weights)


    
