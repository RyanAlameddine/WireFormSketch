# WireformSketch

![fullAdder](https://github.com/RyanAlameddine/WireFormSketch/raw/resources/Demos/fullAdder.gif)

**WireformSketch** is an application that provides functionality for simulating hand drawn digital-logic circuit diagrams! 
The feature detection is implemented purely in OpenCv (no machine learning or neural networks). 
The circuit simulation logic is implemented using [my Wireform library](https://github.com/RyanAlameddine/WireForm).

The algorithm has four layers (outlined in greater detail below):

1) **Document Detection** - detecting and projecting on the bounds of the sheet of paper in the image
2) **Feature Analysis** - detecting and analyzing gate and wire data
    a) **Gate Detection** - detecting and matching expected features of gates
    b) **Wire Detection** - detecting wires and connecting tunnels
3) **Wireform Conversion** - converting detected gates into their Wireform equivalents
4) **Simulation, Output, and Projection**


# Usage

1) Get supplies 
    - One sheet of paper
    - One marker/highligher for gates
    - One marker/highlighter for wires
2) Calibrate Settings
    - Pick the gate/wire/paper colors and camera exposure in WireFormSketch's Settings
3) Draw a diagram!
4) That's it! WireformSketch will handle the rest of the detection and simulation.

Immediately upon detecting the circuit sketch, it will display the Wireform representation in the right debug window.
The input pins (dots) can be toggled by clicking the inputs on this window.

# Demos

### Simple drawing demo:

![fullDraw](https://github.com/RyanAlameddine/WireFormSketch/raw/resources/Demos/fullDraw.gif)

Notice how WireformSketch does not require perfect drawings. 
This algorithm was designed with the intention of working on hand-drawn diagrams (although printed diagrams work too!)

### All-Features Demo:

![allFeatures](https://github.com/RyanAlameddine/WireFormSketch/raw/resources/Demos/allFeatures.gif)

Because WireformSketch is backed by Wireform, it supports all the interesting circuit features Wireform does like cycle-detection, logic gate propogation, state retention!
Also note that due to the capture-based algorithm, WireformSketch is able to retain circuit state and drawing even when the camera is shaking so much that the gates/wires are
almost unrecognizable in the raw image!

# The Algorithm

Here is a quick overview of how the algorithm functions behind the scenes. Note that many details and optimizations are not explained here. 
If you would like to look into my algorithm more deeply, feel free to check out [the source code](https://github.com/RyanAlameddine/WireFormSketch/blob/6c2cd4f66e3ec1824d5c443bf45d1156001d73b6/WireFormSketch/WireFormSketch.cs) and suggest any improvements in a pull request!

For this explanation, we will be working with a single frame from this source image:

<p align="center">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Frame0.png" width="50%">
</p>

## 1) Document Detection

Before we can do anything relating to the gates on the document, we have to detect the paper and project it's bounds to the expected proportions to correct for camera angle.
Because it was not the focus of this application, my document detection algorithm is fairly naive. It consists of the following steps:

#### 1) Blur input to minimize the effects of noise on the outputted image
<p align="center">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Frame.Blurred0.png" width="50%">
</p>

#### 2) Convert to HSV and InRange on the document's expected color
<p align="center">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Frame.Document Mask0.png" width="50%">
</p>

#### 3) Find & Analyze each contour detected and match the one most likely to be the document
<p align="center">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Frame.Document Only Mask0.png" width="50%">
</p>

#### 4) Find the corners of the document and project them to a 8.5/11 image
<p align="center">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Document.Untrimmed0.png" width="40%">
</p>

#### 5) Create an ROI that trims the outermost 10 pixels along the margin to account for frayed edges.

## 2.a) Gate Detection


#### 1) Blur document to minimize the effects of noise on the outputted image
<p align="center">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Document.Blurred0.png" width="50%">
</p>

#### 2) Convert to HSV and InRange on the gate's expected color
<p align="center">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Document.Gate Mask0.png" width="50%">
</p>

#### 3) Dilate the image to combine any skipped edges
<p align="center">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Document.Gate Mask Dilated0.png" width="50%">
</p>

#### 4) Find Contours from mask
<p align="center">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Document.Gate Contours0.png" width="50%">
</p>

#### 5) Isolate each contour and perform trait analysis
<p align="center">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Gates.Contours0.png" width="13%">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Gates.Contours1.png" width="13%">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Gates.Contours2.png" width="13%">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Gates.Contours3.png" width="13%">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Gates.Contours4.png" width="13%">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Gates.Contours5.png" width="13%">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Gates.Contours6.png" width="13%">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Gates.Contours7.png" width="13%">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Gates.Contours8.png" width="13%">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Gates.Contours9.png" width="13%">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Gates.Contours10.png" width="13%">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Gates.Contours11.png" width="13%">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Gates.Contours12.png" width="13%">
</p>

Trait analysis is my algorithm to convert contour data into a `GateTraits` enum. The enum is defined as follows:
```cs
[Flags]
public enum GateTraits
{
//traits:
    NoTraits = 0,

    FilledIn             = 0b00000001, ///0 open regions in contour
    NotDotted            = 0b00000010, ///1 open regions in contour
    Dotted               = 0b00000100, ///2 open regions in contour
    FlatLeftEdge         = 0b00001000, ///the left edge is a flat line
    FirstChildTriangular = 0b00010000, ///the leftmost open region is a triangle
    Bar                  = 0b00100000, ///whether it is to the right of a xor bar

//gates and modifiers:
    Bit    = FilledIn,
    Tunnel = FilledIn | Bar,

    Or  = NotDotted,
    Xor = Or | Bar,
    And = Or | FlatLeftEdge,

    NOr  = Dotted,
    XNor = NOr  | Bar,
    NAnd = NOr  | FlatLeftEdge,
    Not  = NAnd | FirstChildTriangular,
}
```

Notice how this flag-based enum is set up; Each trait corresponds to one bit. The gates and modifiers are defined as compositions of the traits. 

For example, the `XNor` gate trait is defined as a `Nor` with a `Bar`. A `Nor` is defined as `Dotted`. 

Now, these traits are obviously not built into OpenCv. How does the algorithm detect each of them from contour data?

The first three traits refer to the amount of 'child' contours within the target contour. 
If a contour is solid, it will have no children. 
If it is hollow, it will have one child. 
If it is hollow and has a an open 'dot' at the end (like in a Not gate), it will have two children.
These are all easily determined using hierarchical contour data.

The next trait, `FlatLeftEdge`, is determined by applying a polygon approximation algorithm (Ramer–Douglas–Peucker) and then selecting all the points in the contour to the left of the centroid of the contour.
If there are only two such points, the left edge is considered flat (like in an And or Not gate).

The next and most specific trait `FirstChildTriangular`, simply applies the same algorithm to the child contour with the leftmost centroid (importantly the inner edge of a not gate).
It then checks if the polygon approximation output is a triangle.

The final trait is `Bar` which is actually not determined during this step. See the later section on modifiers.


#### 6) For each contour which was not matched to a gate, attempt to match to a modifier

Up to this point, we have only been analyzing individual contours. In some cases (like the bar in an Xor gate), data is contained in multiple contours together.
Essentially, modifiers represent some data about a contour or gate that is contained in a separate contour.
Currently, there are only two types of modifiers: tunnels, and Xor bars. 

For each unused contour from the `GetTraits` step, we do a few checks on shape and other contour properties to determine what the modifier is and apply it to it's respective gate.

For example, with an Xor bar, we follow the following steps:

1) Find the height and centroid of the bar.
2) Take a point of distance `height` to the right of the centroid.
3) Search within a radius `height` around that point and snap the `Bar` modifier onto the nearest gate within that radius.
4) Union the bounding rect of this bar with the bounding rect of the modified gate.

We then get the following modified contour data:

<p align="center">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Gates.Modified1.png" width="19%">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Gates.Modified3.png" width="19%">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Gates.Modified4.png" width="19%">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Gates.Modified6.png" width="19%">
</p>

*Any contour detected that still matches nothing useful is discarded at this point.*

## 2.b) Wire Detection

#### 1) Convert blurred document mask to HSV and InRange on the wires's expected color
<p align="center">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Document.Wire Mask0.png" width="50%">
</p>

#### 2) Find the wire contours
<p align="center">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/Document.Wire Contours0.png" width="50%">
</p>

#### 3) Approximate the wire contours to a polygon to minimize the amount of unneccessary points

Once that step is complete, we now have completed the OpenCv detection layer. Here is a debugging output of our current state:
<p align="center">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/outCv.png" width="60%">
</p>

## 3) Wireform Conversion

The wireform conversion has two (fairly) simple steps.

#### 1) Convert gates to their corresponding Wireform representation

Each detected gate is matched based on it's traits to a Wireform class. 
The positions of its inputs, outputs, and hitbox are then updated procedurally to match their expected positions in the sketch.

1.a) For each tunnel, simply add a wire from one input to the next that skips over all the wires between.

#### 2) Add and snap wires

Each segment of interest in each wire is added to the board state. 

Then, the iterate over each gate input and output (known as `GatePin`s). For each pin, search for the nearest wire point within a certain radius. 
If one is found, snap it to the pin and register the connection!

#### Wireform is loaded!

You can see the resulting output below:

<p align="center">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/outWireform.png" width="60%">
</p>

Green +'s are inputs. Green diamonds are outputs. Yellow +'s are connections.

## 4) Simulation, Output, and Projection

As soon as this process is complete, we can request wireform propogate electricity through the circuit and display it on screen!

To do that, we simply need to draw the wire-value data over the corresponding contours in the document image. 
Then, we can invert the perspective transformation matrix we originally used to map the document contour to the document image to undo that transformation and project back onto the original image!

Our process is complete!

<p align="center">
  <img src="https://github.com/RyanAlameddine/WireFormSketch/raw/resources/images/out.png" width="80%">
</p>

