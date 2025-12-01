import cv2 as cv
import numpy as np

img = cv.imread('kfs_r1.png')
hsv = cv.cvtColor(img, cv.COLOR_BGR2HSV)

RED = (222, 7, 22)
BLUE = (28, 33, 135)

RED_HSV = cv.cvtColor(np.uint8([[RED]]), cv.COLOR_RGB2HSV)[0,0]
BLUE_HSV = cv.cvtColor(np.uint8([[BLUE]]), cv.COLOR_RGB2HSV)[0,0]

LOWER_VARIANCE = np.array([18, 100, 255])
UPPER_VARIANCE = np.array([18, 10, 20])

print(RED_HSV, BLUE_HSV)

red_mask = cv.inRange(hsv, RED_HSV - LOWER_VARIANCE, RED_HSV + UPPER_VARIANCE)
if RED_HSV[0] < 90:
    # is in lower half
    red_mask = red_mask | cv.inRange(hsv, RED_HSV - LOWER_VARIANCE + [180, 0, 0], RED_HSV + UPPER_VARIANCE + [180, 0, 0])
else:
    # is in upper half
    red_mask = red_mask | cv.inRange(hsv, RED_HSV - LOWER_VARIANCE - [180, 0, 0], RED_HSV + UPPER_VARIANCE - [180, 0, 0])
blue_mask = cv.inRange(hsv, BLUE_HSV - LOWER_VARIANCE, BLUE_HSV + UPPER_VARIANCE)

kernel = cv.getStructuringElement(cv.MORPH_ELLIPSE, (7, 7))
red_mask = cv.morphologyEx(red_mask, cv.MORPH_OPEN, kernel)
blue_mask = cv.morphologyEx(blue_mask, cv.MORPH_OPEN, kernel)

red_mask = np.repeat(red_mask[...,None], 3, axis=2)
blue_mask = np.repeat(blue_mask[...,None], 3, axis=2)

red_to_blue = (hsv + (BLUE_HSV - RED_HSV)) & red_mask
blue_to_red = (hsv + (RED_HSV - BLUE_HSV)) & blue_mask

final_hsv = red_to_blue | blue_to_red | (hsv & ~(red_mask | blue_mask))
result = cv.cvtColor(final_hsv.astype(np.uint8), cv.COLOR_HSV2BGR)

cv.imshow("Original", img)
cv.imshow("Mask", img & (red_mask | blue_mask))
cv.imshow("Result", result)
try:
    while True:
        if cv.getWindowProperty("Result", cv.WND_PROP_VISIBLE) < 1:
            break
        cv.waitKey(1)
except KeyboardInterrupt:
    pass
