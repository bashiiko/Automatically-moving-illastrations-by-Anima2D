# TODO: コーナー検出の閾値どうするか
# TODO: コーナー絞り込みの2回めのとこを直す
# TODO: 交点の場合分けが怪しいようなあってるような
"""
Input
    画像ファイル
Outpt
    透過画像
        Inputと同じ名前(上書き)
    Boneの始点・終点が書かれたCSVファイル
        points.csv(実行の都度上書き)
"""
import sys

import cv2
import numpy as np
import pandas as pd

import sympy as sym
import os
import matplotlib.pyplot as plt
import random
import pathlib


(x, y) = sym.symbols("x y", real=True)

# init
filename = sys.argv[1]

# 画像read, BGR
img = cv2.imread(filename, 1)
img = cv2.resize(img, (img.shape[1] // 2, img.shape[0] // 2))

# before color
white = (img[..., 0] == 255) & (img[..., 1] == 255) & (img[..., 2] == 255)
c_a = [random.randint(200,255), random.randint(200,255), random.randint(200,255)]
img[white] =  np.array(c_a, dtype=np.uint8)

# マスク作成, Gray
mask = np.zeros((img.shape[0]+2, img.shape[1]+2), dtype=np.uint8)
_, _, mask, _ = cv2.floodFill(img, mask, seedPoint=(0,0), newVal=c_a)

# アルファチャンネル作成
# maskは元画像+2pxのサイズだから削る
alpha = mask.copy()
alpha[alpha == 0] = 255
alpha[alpha == 1] = 0
alpha = np.delete(alpha, [0,alpha.shape[0]-1], 0)
alpha = np.delete(alpha, [0,alpha.shape[1]-1], 1)
a_img = cv2.merge((img, alpha))


# 透過画像書き出し
cv2.imwrite(filename, a_img)

# 2値化 maskじゃなくてalphaの配色じゃないとだめらしい
_, bin_img = cv2.threshold(alpha, 127, 255, 0)

# マスクの輪郭抽出(元画像から直接抽出すると取れない場合がある)
contours, hierarchy = cv2.findContours(bin_img, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)

# blank image
blank_img = np.zeros((img.shape[0], img.shape[1], 3), np.uint8)
blank_img += 255

# 一番長い輪郭
max_cnt = contours[0]
for cnt in contours:
    if len(cnt) > len(max_cnt):
        max_cnt = cnt

# 輪郭画像を作る
cnt_img = blank_img.copy()
cv2.drawContours(cnt_img, [max_cnt], 0, (0,255,0), 5)

base = os.getcwd()
# Unityから呼び出すとassetsが含まれない
cnt_img_a = cv2.merge((cnt_img, alpha))
filepath = base +"\\Assets\\Resources\\img_processing1.png"
cv2.imwrite(filepath, cnt_img_a )

# 重心計算
mu = cv2.moments(max_cnt)
cog_x = int(mu["m10"] / mu["m00"])
cog_y = int(mu["m01"] / mu["m00"])

# 最小内接円っぽいやつの半径を探す
# 初期値, 適当
r_min = int(np.sqrt(cnt_img.shape[0]**2 + cnt_img.shape[1]**2))
for p in max_cnt:
    tmp = int(np.sqrt((p[0][0] - cog_x)**2 + (p[0][1] - cog_y)**2))
    if r_min > tmp:
        r_min = tmp

# コーナー検出
tmp = cv2.cvtColor(cnt_img, cv2.COLOR_BGR2GRAY)
dst = cv2.cornerHarris(np.float32(tmp), 2, 3, 0.04)
dst = cv2.dilate(dst, None)
# コーナーy座標, x座標(row, col) 閾値任意
corner_y, corner_x = np.where(dst>0.1*dst.max())  # 閾値どうするか

#cnt_img[corner_y,corner_x] =[204,50,153];
for i in range(len(corner_x)):
    cv2.circle(cnt_img,(corner_x[i],corner_y[i]),5,(204,50,153),thickness=-1)
cnt_img_a = cv2.merge((cnt_img, alpha))
filepath = base +"\\Assets\\Resources\\img_processing2.png"
cv2.imwrite(filepath, cnt_img_a)

# とりあえず10*10分割の窓
window_x = cnt_img.shape[1] // 10
window_y = cnt_img.shape[0] // 10

# indexが必要なのでSeries
corner_x_s = pd.Series(corner_x)
corner_y_s = pd.Series(corner_y)

# mean point
mp_x = []
mp_y = []

# 単一窓内ごとの要約
for i in range(10):
    for j in range(10):
        tmp_x = corner_x_s[((window_x*i) < corner_x_s) & (corner_x_s < (window_x*(i+1)))].index
        tmp_y = corner_y_s[((window_y*j) < corner_y_s) & (corner_y_s < (window_y*(j+1)))].index
        tmp_index = [idx for idx in tmp_x.values if idx in tmp_y.values]

        if tmp_index:
            mp_x.append(int(corner_x_s[tmp_index].mean()))
            mp_y.append(int(corner_y_s[tmp_index].mean()))

# ここ適当なのであんまこれでやりたくない
# 5*5分割の窓
window_x = cnt_img.shape[1] // 5
window_y = cnt_img.shape[0] // 5

# indexが必要なのでSeries
mp_x_s = pd.Series(mp_x)
mp_y_s = pd.Series(mp_y)

# mean point
mp_x = []
mp_y = []

# 単一窓内ごとの要約
for i in range(5):
    for j in range(5):
        tmp_x = mp_x_s[((window_x*i) < mp_x_s) & (mp_x_s < (window_x*(i+1)))].index
        tmp_y = mp_y_s[((window_y*j) < mp_y_s) & (mp_y_s < (window_y*(j+1)))].index
        tmp_index = [idx for idx in tmp_x.values if idx in tmp_y.values]

        if tmp_index:
            mp_x.append(int(mp_x_s[tmp_index].mean()))
            mp_y.append(int(mp_y_s[tmp_index].mean()))

# 要約した頂点：可視化のための処理

for i in range(len(mp_x)):
    cv2.circle(cnt_img,(mp_x[i],mp_y[i]),5,[0,0,255],thickness=-1)
cnt_img_a = cv2.merge((cnt_img, alpha))
filepath = base +"\\Assets\\Resources\\img_processing3.png"
cv2.imwrite(filepath, cnt_img_a)

# 重心と絞られたコーナーの距離計算
distance = pd.DataFrame({"x" : mp_x, "y" : mp_y})
distance["d"] = (cog_x-distance["x"])**2 + (cog_y-distance["y"])**2

# ここでBoneの本数を決定
distance = distance.sort_values("d", ascending=False).head(5)

# 交点保存用
isec_x_list = []
isec_y_list = []

# 内接円とコーナーの交点(=bone始点)を計算する
for index, row in distance.iterrows():
    # 交点
    F1 = (x-cog_x)**2 + (y-cog_y)**2 - r_min**2
    F2 = (row["x"]-cog_x)*(y-cog_y) - (row["y"]-cog_y)*(x-cog_x)
    solve = sym.solve([F1, F2], [x, y])

    # 交点場合分け
    ########## ここから怪しい
    isec_x1 = int(sym.N(solve[0][0]))
    isec_x2 = int(sym.N(solve[1][0]))
    if cog_x > row["x"]:
        if isec_x1 > cog_x:
            isec_x = isec_x2
            isec_y = int(sym.N(solve[1][1]))
        else:
            isec_x = isec_x1
            isec_y = int(sym.N(solve[0][1]))
    if cog_x < row["x"]:
        if isec_x1 > cog_x:
            isec_x = isec_x1
            isec_y = int(sym.N(solve[0][1]))
        else:
            isec_x = isec_x2
            isec_y = int(sym.N(solve[1][1]))
    ########## ここまで怪しい
    cnt_img = cv2.line(cnt_img,(isec_x,isec_y),(row["x"],row["y"]),(0,0,0),2)
    isec_x_list.append(isec_x)
    isec_y_list.append(isec_y)

# 可視化時に座標が反転？するため．よくわからん
#for i in range(0,len(distance)-1):

cnt_img_a = cv2.merge((cnt_img, alpha))
filepath = base +"\\Assets\\Resources\\img_processing4.png"
cv2.imwrite(filepath, cnt_img_a)

# csvに座標保存
points = distance.copy().drop("d", axis=1)
if r_min * 1.5 > np.sqrt(distance["d"].min()):  # 内接円円周と重心が十分離れていない場合
    point["x_2"] = cog_x
    point["y_2"] = cog_y
else:
    points["x_2"] = isec_x_list
    points["y_2"] = isec_y_list
# Unityから呼び出すとassetsが含まれない
points.T.to_csv(base +"\\Assets\\Resources\\points.csv")
#print(base +"\\Resources\\CSV\\points.csv")
