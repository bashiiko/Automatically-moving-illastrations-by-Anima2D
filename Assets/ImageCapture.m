function varargout = OLdemo3(varargin)
% OLDEMO3 MATLAB code for OLdemo3.fig
%      OLDEMO3, by itself, creates a new OLDEMO3 or raises the existing
%      singleton*.
%
%      H = OLDEMO3 returns the handle to a new OLDEMO3 or the handle to
%      the existing singleton*.
% 
%      OLDEMO3('CALLBACK',hObject,eventData,handles,...) calls the local
%      function named CALLBACK in OLDEMO3.M with the given input arguments.
%
%      OLDEMO3('Property','Value',...) creates a new OLDEMO3 or raises the
%      existing singleton*.  Starting from the left, property value pairs are
%      applied to the GUI before OLdemo3_OpeningFcn gets called.  An
%      unrecognized property name or invalid value makes property application
%      stop.  All inputs are passed to OLdemo3_OpeningFcn via varargin.
%
%      *See GUI Options on GUIDE's Tools menu.  Choose "GUI allows only one
%      instance to run (singleton)".
%
% See also: GUIDE, GUIDATA, GUIHANDLES

% Edit the above text to modify the response to help OLdemo3

% Last Modified by GUIDE v2.5 14-May-2019 19:48:05

% Begin initialization code - DO NOT EDIT
gui_Singleton = 1;
gui_State = struct('gui_Name',       mfilename, ...
                   'gui_Singleton',  gui_Singleton, ...
                   'gui_OpeningFcn', @OLdemo3_OpeningFcn, ...
                   'gui_OutputFcn',  @OLdemo3_OutputFcn, ...
                   'gui_LayoutFcn',  [] , ...
                   'gui_Callback',   []);
if nargin && ischar(varargin{1})
    gui_State.gui_Callback = str2func(varargin{1});
end

if nargout
    [varargout{1:nargout}] = gui_mainfcn(gui_State, varargin{:});
else
    gui_mainfcn(gui_State, varargin{:});
end
% End initialization code - DO NOT EDIT


% --- Executes just before OLdemo3 is made visible.
function OLdemo3_OpeningFcn(hObject, eventdata, handles, varargin)
% This function has no output args, see OutputFcn.
% hObject    handle to figure
% eventdata  reserved - to be defined in a future version of MATLAB
% handles    structure with handles and user data (see GUIDATA)
% varargin   command line arguments to OLdemo3 (see VARARGIN)
handles.h1=findobj('Type','axes','Tag','axes1');
handles.h2=findobj('Type','axes','Tag','axes2');
handles.mycam=webcam('C922 Pro Stream Webcam');
handles.mycam.Zoom=100;
handles.zoom=handles.mycam.Zoom;
preview(handles.mycam)
% Choose default command line output for OLdemo3
handles.output = hObject;

% Update handles structure
guidata(hObject, handles);

% UIWAIT makes OLdemo3 wait for user response (see UIRESUME)
% uiwait(handles.figure1);


% --- Outputs from this function are returned to the command line.
function varargout = OLdemo3_OutputFcn(hObject, eventdata, handles)
% varargout  cell array for returning output args (see VARARGOUT);
% hObject    handle to figure
% eventdata  reserved - to be defined in a future version of MATLAB
% handles    structure with handles and user data (see GUIDATA)

% Get default command line output from handles structure
varargout{1} = handles.output;


% --- Executes on button press in pushbutton1.
function pushbutton1_Callback(hObject, eventdata, handles) %?øΩB?øΩe?øΩ{?øΩ^?øΩ?øΩ?øΩÃëÔøΩ?øΩ?øΩ
% hObject    handle to pushbutton1 (see GCBO)
% eventdata  reserved - to be defined in a future version of MATLAB
% handles    structure with handles and user data (see GUIDATA)
handles.Img=snapshot(handles.mycam); %?øΩJ?øΩ?øΩ?øΩ?øΩ?øΩÊëúÔøΩ?øΩ?øΩB?øΩe
axes(handles.h1); %?øΩ\?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩW?øΩ?øΩ?øΩ?øΩ?øΩI?øΩ?øΩ
imshow(handles.Img) %?øΩB?øΩe?øΩ?øΩ?øΩ?øΩ?øΩÊëúÔøΩ?øΩ?øΩ\?øΩ?øΩ
guidata(hObject, handles);


% --- Executes on button press in pushbutton2.
function pushbutton2_Callback(hObject, eventdata, handles) %?øΩÿÇËî≤?øΩ?øΩ?øΩ{?øΩ^?øΩ?øΩ?øΩÃëÔøΩ?øΩ?øΩ
% hObject    handle to pushbutton2 (see GCBO)
% eventdata  reserved - to be defined in a future version of MATLAB
% handles    structure with handles and user data (see GUIDATA)
extracted=OLDext(handles.Img); %?øΩC?øΩ?øΩ?øΩX?øΩg?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩÿÇÔøΩ?øΩ?øΩ?øΩ?øΩ
handles.extImg=extracted{1}; %?øΩÿÇÔøΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩC?øΩ?øΩ?øΩX?øΩg
handles.binary=extracted{2}; %?øΩC?øΩ?øΩ?øΩX?øΩg?øΩÃÉo?øΩC?øΩi?øΩ?øΩ
axes(handles.h2);
imshow(handles.extImg)
guidata(hObject, handles);


% --- Executes on button press in pushbutton3.
function pushbutton3_Callback(hObject, eventdata, handles) %?øΩ€ëÔøΩ?øΩ{?øΩ^?øΩ?øΩ?øΩÃëÔøΩ?øΩ?øΩ
% hObject    handle to pushbutton3 (see GCBO)
% eventdata  reserved - to be defined in a future version of MATLAB
% handles    structure with handles and user data (see GUIDATA)
files=dir('./Sprites/*.png');
%filenumber=(length(files)/3)+1; %?øΩw?øΩËÇµ?øΩ?øΩ?øΩt?øΩH?øΩ?øΩ?øΩ_?øΩ?øΩpng?øΩt?øΩ@?øΩC?øΩ?øΩ?øΩÃêÔøΩ
filenumber=(length(files))+1; 
filenumber_c=num2str(filenumber);
head1='./Sprites/ext';
head2='./Sprites/target';
head3='./Sprites/pic';
foot='.png';
filename1=strcat(head1,filenumber_c,foot);
filename2=strcat(head2,filenumber_c,foot);
filename3=strcat(head3,filenumber_c,foot);
%imwrite(handles.extImg,filename1) %?øΩ?øΩ?øΩo?øΩC?øΩ?øΩ?øΩX?øΩg?øΩ?øΩ?øΩ€ëÔøΩ
imwrite(handles.binary,filename2) %?øΩo?øΩC?øΩi?øΩ?øΩ?øΩ?øΩ?øΩ€ëÔøΩ
%imwrite(handles.Img,filename3) %?øΩ?øΩ?øΩÃâÊëú?øΩ?øΩ?øΩ€ëÔøΩ
guidata(hObject,handles);


% --- Executes on slider movement.
function slider1_Callback(hObject, eventdata, handles) %?øΩX?øΩ?øΩ?øΩC?øΩ_?øΩ[?øΩ≈î{?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ
% hObject    handle to slider1 (see GCBO)
% eventdata  reserved - to be defined in a future version of MATLAB
% handles    structure with handles and user data (see GUIDATA)
slider_value=get(hObject,'Value');
handles.mycam.Zoom=slider_value;
guidata(hObject,handles);

% Hints: get(hObject,'Value') returns position of slider
%        get(hObject,'Min') and get(hObject,'Max') to determine range of slider


% --- Executes during object creation, after setting all properties.
function slider1_CreateFcn(hObject, eventdata, handles)
% hObject    handle to slider1 (see GCBO)
% eventdata  reserved - to be defined in a future version of MATLAB
% handles    empty - handles not created until after all CreateFcns called

% Hint: slider controls usually have a light gray background.
if isequal(get(hObject,'BackgroundColor'), get(0,'defaultUicontrolBackgroundColor'))
    set(hObject,'BackgroundColor',[.9 .9 .9]);
end

function Y = OLDext(IMG)
IMG_g=rgb2gray(IMG);
picture=IMG;
IMG_bi=imbinarize(IMG_g,'adaptive','ForegroundPolarity','dark','Sensitivity',0.4); %?øΩ?øΩ?øΩl?øΩ?øΩ
IMG_bifill=imfill(~IMG_bi,'holes'); %?øΩy?øΩ?øΩ?øΩ≈àÕÇ‹ÇÍÇΩ?øΩ?øΩ?øΩ?øΩ?øΩñÑÇﬂÇÔøΩ?øΩC?øΩ?øΩ?øΩX?øΩg?øΩÃÉo?øΩC?øΩi?øΩ?øΩ?øΩ}?øΩX?øΩN
IMG_bi3=cat(3,IMG_bifill,IMG_bifill,IMG_bifill); %3?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ
picture(IMG_bi3==0)=0;
answer={picture,IMG_bi};
Y=answer;
