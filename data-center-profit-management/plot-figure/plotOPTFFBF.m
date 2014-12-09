function plotOPTFFBF(metrixType, arrivalrate, setting)
    %Build Figure
    setting = '48-16-10';
    
    opt = [578.1, 1631.8 1 2741.6  2860.0  3154.5   ]
    ff = [461.6, 1287.8  1 2263.8   2619.5 2964.5 ]
    bf = [535.8, 1557.6  1 2398.4 2641.0 2920.0  ]



    figure1 = figure;
    set(figure1,'units','normalized','outerposition',[0 0 1 1], 'visible','on');

    axes1 = axes('Parent',figure1);
    box(axes1,'on');
    hold(axes1,'all');
    set(axes1,'FontSize',30,'FontWeight','bold');


    %set x,y axis
    x=[0.2 0.6 0.8 1.0 1.2 1.4]
    
    %plot and set font, line type
    p = plot(x(1,:),opt(1,:));
    set(p, 'Color', 'r', 'LineWidth', 3, 'linestyle','--');
    set(p, 'Marker', 's', 'MarkerSize', 10);

    %plot and set font, line type
    p = plot(x(1,:),ff(1,:));
    set(p, 'Color', 'b', 'LineWidth', 3, 'linestyle','--');
    set(p, 'Marker', '<', 'MarkerSize', 10);

    p = plot(x(1,:),bf(1,:));
    set(p, 'Color', 'g', 'LineWidth', 3, 'linestyle','--');
    set(p, 'Marker', '*', 'MarkerSize', 10);
        

%     errorbar(x,opt(1,:),optErr(1,:),'Color', 'r','LineWidth', 2 )
%     errorbar(x,ff(1,:),ffErr(1,:),'Color', 'b','LineWidth', 2 )
% 
%     errorbar(x,bf(1,:),bfErr(1,:),'Color', 'g','LineWidth', 2)


    %set x range and y range
    upper = max( max(bf(1,:), max(max(opt(1,:)) ,max(ff(1,:)) )))

     upper = ceil(upper/100)*100;        
 
         
    axis([0.19 1.4 0 upper])  
    

    %set x tick and y tick
    %set(axes1,'YTick',[0.2,0.4,0.6,0.8,1.0],'YTickLabel',{'20%','40%','60%','80%','100%'},'XGrid','on','YGrid','on');
   set(axes1,'XTick',[0.2 0.6 0.8 1.0 1.2 1.4],'XTickLabel',[0.2 0.6 0.8 1.0 1.2 1.4],'XGrid','on','YGrid','on');

    %set grid on 
    set(axes1,'XGrid','on','YGrid','on');

    %set legend
   

        legend(axes1,'show','Location','SouthEast','FontSize',10,'FontWeight','bold');
        leg=    legend('OPT', 'FirstFit','BestFit');
        set(leg,'Location','SouthEast');

    %set x, y Label
    set(get(axes1,'XLabel'),'String','Arrival rate','FontSize',30,'FontWeight','bold');
    set(get(axes1,'YLabel'),'String', 'Profit','FontSize',30,'FontWeight','bold');

    %set title
    %title('10000 Attackers, 1000 Proxies');
    set(gcf, 'PaperPosition', [0 0 13 7]); %Position plot at left hand corner with width 5 and height 5.
    set(gcf, 'PaperSize', [13 7]); %Set the paper to have width 5 and height 5.
    %saveas(gcf, 'SolarTrace_High', 'pdf') %Save figure
    filename = strcat('.\figures\OPTFFBF_',setting)
    saveas(gcf, filename, 'pdf') %Save figure  
    saveas(gca, strcat(filename, '.eps'),'psc2') %Save figure 
    
     filename = strcat('C:\Users\hwang14\Dropbox\Meerkats\greenSlot\greenSlot-CS773\Figures\OPTFFBF_', setting)
    saveas(gcf, filename, 'pdf') %Save figure  
    saveas(gca, strcat(filename, '.eps'),'psc2') %Save figure 
    
end
 