%Build Figure



figure1 = figure;
set(figure1,'units','normalized','outerposition',[0.683950 2.049900 6.847000 7.713800]);

axes1 = axes('Parent',figure1);
box(axes1,'on');
hold(axes1,'all');
set(axes1,'FontSize',30,'FontWeight','bold');



[x y] = textread('..\data\z.txt');
 
line( [0.683950,0.683950, ],[ 2.049900 , 7.713800],'linewidth' ,3)
line( [3.610804,3.610804 ],[ 2.049900 , 7.713800],'linewidth' ,3)
line( [4.698594,4.698594 ],[ 2.049900 , 7.713800],'linewidth' ,3)
line( [6.847000,6.847000 ],[ 2.049900 , 7.713800],'linewidth' ,3)

line([ 0.683950 ,  6.847000], [ 2.049900, 2.049900],'Color', 'g', 'linewidth' ,3)
line([ 0.683950 ,  6.847000], [ 4.408182,4.408182 ],'Color', 'g', 'linewidth' ,3)
line([ 0.683950 ,  6.847000], [  5.567526, 5.567526],'Color', 'g', 'linewidth' ,3)
line([ 0.683950 ,  6.847000], [7.713800,7.713800],'Color', 'g', 'linewidth' ,3)

% line([ 0.683950 ,  6.847000], [])




% line( [4.792650,2.049900 ],[4.792650 , 7.713800])
 

plot(x(1:99,1),y(1:99,1), 'r.','MarkerSize', 20, 'linewidth' ,3 );
hold('on');
% plot(x(100,1),y(100,1), 'b*','MarkerSize', 12, 'linewidth' ,3);
% plot(x(101,1),y(101,1), 'go','MarkerSize', 12,'linewidth' ,3);
% plot(x(102,1),y(102,1), 'ms','MarkerSize', 12,'linewidth' ,3);
% plot(3.965,4.960,  'k>','MarkerSize', 12,'linewidth' ,3)

%set x range and y range
axis([0.683950 6.847000  2.049900  7.713800])  

%set x tick and y tick

 
%set grid on 
set(axes1,'XGrid','off','YGrid','off');
    


%set x, y Label
set(get(axes1,'XLabel'),'String','x','FontSize',30,'FontWeight','bold');
set(get(axes1,'YLabel'),'String','y','FontSize',30,'FontWeight','bold');

%set title
%title('10000 Attackers, 1000 Proxies');

%save figure
set(gcf, 'PaperPosition', [0 0 13 7]); %Position plot at left hand corner with width 5 and height 5.
set(gcf, 'PaperSize', [13 7]); %Set the paper to have width 5 and height 5.



saveas(gca, strcat('..\figs\equal-depth', '.eps'),'psc2') %Save figure 
saveas(gcf, strcat('..\figs\equal-depth'), 'pdf') %Save figure






