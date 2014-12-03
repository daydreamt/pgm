module file1testblabla
(* module Charts =
    open FSharp.Charting

    Chart.Line([ for x in 0 .. 10 -> x, x*x ]).ShowChart()
    *)

open MathNet.Numerics
SpecialFunctions.Gamma(0.5)

open MathNet.Numerics.LinearAlgebra
open System.Numerics

open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Double
open MathNet.Numerics.Distributions


open MathNet.Numerics.LinearAlgebra.Double

let b2 = DenseMatrix.create 3 4 20.5


let b3 : float Matrix = SparseMatrix.zero 3 4


open MathNet.Numerics.LinearAlgebra
let m : Matrix<float> = DenseMatrix.randomStandard 50 50
(m * m.Transpose()).Determinant()

