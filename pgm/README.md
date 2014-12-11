pgm is a postmodern graphical model library, proudly provided by poeticaexmachina.org

It aims at nothing less than being the worlds best graphic model library.

It includes undirected models, (with special emphasis on factor graphs), as well as directed models.
All training algorithms are of course GPU powered.

LOL jk

#####################################################################################
Rationale

The main design goal of this library is that it should make me happy, not be perfect.
Being in a nice language helps. Having many different models and being flexible helps. We can optimise later.
I am also hoping to finally settle on a platform I like so that I can do research with it.

So, that's that.

#####################################################################################

  The library's main idea is that all probabilistic variables can be represented as a node
in a graphical model. 


The main types are Models

####################################################################################
Dependencies
MathNet.Numerics.FSharp
QuickGraph


#######################################################################################
    ROADMAP
1) Integrate a graph library [4/10]
2) Allow random variables = Vertices with distributions [active]
    2.Future) Automatically use the MathNet.Numerics.Distributions?
3) Make simple factor graphs [3/10]
Simple Message Passing

####################################################################################
LICENSE: TODO: probably LGPL3 if compatible, and another one on demand