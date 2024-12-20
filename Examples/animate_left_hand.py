import imumocap
import models
import numpy as np
import time
from imumocap import Matrix

# Load example model
model = models.LeftHand()

# Create animation frames
frames = []

for angle in [np.sin(x) for x in np.linspace(0, np.pi, 100)]:
    model.left_first_distal.joint = model.left_first_transformation(Matrix(rot_x=-45 * angle))
    model.left_first_proximal.joint = model.left_first_transformation(Matrix(rot_x=-45 * angle))
    model.left_first_metacarpal.joint = model.left_first_transformation(Matrix(rot_x=-45 * angle))

    model.left_second_distal.joint = Matrix(rot_x=-90 * angle)
    model.left_second_middle.joint = Matrix(rot_x=-90 * angle)
    model.left_second_proximal.joint = Matrix(rot_x=-90 * angle)

    model.left_third_distal.joint = Matrix(rot_x=-90 * angle)
    model.left_third_middle.joint = Matrix(rot_x=-90 * angle)
    model.left_third_proximal.joint = Matrix(rot_x=-90 * angle)

    model.left_forth_distal.joint = Matrix(rot_x=-90 * angle)
    model.left_forth_middle.joint = Matrix(rot_x=-90 * angle)
    model.left_forth_proximal.joint = Matrix(rot_x=-90 * angle)

    model.left_fifth_distal.joint = Matrix(rot_x=-90 * angle)
    model.left_fifth_middle.joint = Matrix(rot_x=-90 * angle)
    model.left_fifth_proximal.joint = Matrix(rot_x=-90 * angle)

    frames.append({l.name: l.joint for l in model.root.flatten()})  # each frame is a dictionary of joint matrices

# Plot
model.root.plot(frames)

# Stream to UI
connection = imumocap.ui.Connection()

while True:
    for frame in frames:
        time.sleep(1 / 30)  # 30 fps

        for name, joint in frame.items():
            model.root.dictionary()[name].joint = joint

        connection.send(imumocap.ui.link_to_primitives(model.root))