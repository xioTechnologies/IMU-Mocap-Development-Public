import imumocap
import imumocap.solvers
import models
import numpy as np
import time
from imumocap import Matrix

# Load example model
model = models.Body()

# Create animation frames
frames = []

for angle in [np.sin(x) for x in np.linspace(0, np.pi, 100)]:
    model.head.joint = Matrix(rot_y=angle * 15)
    model.neck.joint = Matrix(rot_y=angle * 15)

    model.left_hand.joint = Matrix(rot_x=angle * -45)
    model.left_forearm.joint = Matrix(rot_y=angle * -90, rot_z=angle * -60)
    model.left_upper_arm.joint = Matrix(rot_x=angle * 10, rot_z=angle * -60)
    model.left_shoulder.joint = Matrix(rot_x=angle * -10)

    model.right_hand.joint = Matrix(rot_x=angle * 45)
    model.right_forearm.joint = Matrix(rot_y=angle * -90, rot_z=angle * 60)
    model.right_upper_arm.joint = Matrix(rot_x=angle * -10, rot_z=angle * 60)
    model.right_shoulder.joint = Matrix(rot_x=angle * -10)

    model.upper_torso.joint = Matrix(rot_y=angle * 15)
    model.lower_torso.joint = Matrix(rot_y=angle * 15)
    model.upper_lumbar.joint = Matrix(rot_y=angle * 15)
    model.lower_lumbar.joint = Matrix(rot_y=angle * 15)

    model.left_toe.joint = Matrix(rot_y=angle * -45)
    model.left_foot.joint = Matrix(rot_y=angle * -45)
    model.left_lower_leg.joint = Matrix(rot_y=angle * 160)
    model.left_upper_leg.joint = Matrix(rot_y=angle * -45)

    model.right_toe.joint = Matrix(rot_y=angle * -45)
    model.right_foot.joint = Matrix(rot_y=angle * -45)
    model.right_lower_leg.joint = Matrix(rot_y=angle * 160)
    model.right_upper_leg.joint = Matrix(rot_y=angle * -45)

    model.pelvis.joint = Matrix(rot_y=angle * -25)

    imumocap.solvers.floor(model.root)

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
