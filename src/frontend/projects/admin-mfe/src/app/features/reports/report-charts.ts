import {
  ApexAxisChartSeries,
  ApexChart,
  ApexDataLabels,
  ApexFill,
  ApexGrid,
  ApexLegend,
  ApexNonAxisChartSeries,
  ApexPlotOptions,
  ApexStroke,
  ApexTooltip,
  ApexXAxis,
  ApexYAxis,
} from 'ng-apexcharts';

/** Materio / CRM brand palette for report charts. */
export const REPORT_COLORS = [
  '#8c57ff',
  '#16b1ff',
  '#56ca00',
  '#ffb400',
  '#ff4c51',
  '#32baff',
  '#9155fd',
  '#26c6da',
];

export type DonutChartOptions = {
  series: ApexNonAxisChartSeries;
  chart: ApexChart;
  labels: string[];
  colors: string[];
  legend: ApexLegend;
  dataLabels: ApexDataLabels;
  plotOptions: ApexPlotOptions;
  stroke: ApexStroke;
  tooltip: ApexTooltip;
};

export type BarChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  yaxis: ApexYAxis;
  colors: string[];
  dataLabels: ApexDataLabels;
  plotOptions: ApexPlotOptions;
  grid: ApexGrid;
  fill: ApexFill;
  tooltip: ApexTooltip;
  legend: ApexLegend;
  stroke: ApexStroke;
};

export type RadialChartOptions = {
  series: ApexNonAxisChartSeries;
  chart: ApexChart;
  labels: string[];
  colors: string[];
  plotOptions: ApexPlotOptions;
  fill: ApexFill;
  stroke: ApexStroke;
};

const baseFont = {
  fontFamily: 'inherit',
};

export function emptyDonut(label = 'No data'): DonutChartOptions {
  return donutChart([1], [label], ['#e7e3fc']);
}

export function donutChart(values: number[], labels: string[], colors = REPORT_COLORS): DonutChartOptions {
  const safe = values.length ? values : [1];
  const safeLabels = labels.length ? labels : ['No data'];
  return {
    series: safe,
    chart: { type: 'donut', height: 300, ...baseFont, toolbar: { show: false } },
    labels: safeLabels,
    colors: colors.slice(0, Math.max(safe.length, 1)),
    legend: {
      position: 'bottom',
      fontSize: '13px',
    },
    dataLabels: { enabled: true, style: { fontSize: '12px', fontWeight: 600 } },
    plotOptions: {
      pie: {
        donut: {
          size: '68%',
          labels: {
            show: true,
            total: {
              show: true,
              label: 'Total',
              formatter: (w) =>
                String(
                  (w.globals.seriesTotals as number[]).reduce((a, b) => a + b, 0),
                ),
            },
          },
        },
      },
    },
    stroke: { width: 2, colors: ['#fff'] },
    tooltip: { y: { formatter: (v) => `${v}` } },
  };
}

export function barChart(
  categories: string[],
  data: number[],
  opts?: { horizontal?: boolean; color?: string; name?: string },
): BarChartOptions {
  const horizontal = opts?.horizontal ?? false;
  return {
    series: [{ name: opts?.name ?? 'Count', data: data.length ? data : [0] }],
    chart: {
      type: 'bar',
      height: horizontal ? Math.max(260, (categories.length || 1) * 42 + 60) : 300,
      toolbar: { show: false },
      ...baseFont,
    },
    plotOptions: {
      bar: {
        horizontal,
        borderRadius: 6,
        columnWidth: '48%',
        barHeight: '58%',
        distributed: !opts?.name,
      },
    },
    colors: opts?.color ? [opts.color] : REPORT_COLORS,
    dataLabels: {
      enabled: true,
      style: { fontSize: '11px', fontWeight: 600, colors: ['#5d596c'] },
      offsetY: horizontal ? 0 : -6,
    },
    xaxis: {
      categories: categories.length ? categories : ['—'],
      labels: { style: { colors: '#a5a3ae', fontSize: '12px' } },
      axisBorder: { show: false },
      axisTicks: { show: false },
    },
    yaxis: {
      labels: { style: { colors: '#a5a3ae', fontSize: '12px' } },
    },
    grid: {
      borderColor: 'rgba(47, 43, 61, 0.08)',
      strokeDashArray: 4,
      xaxis: { lines: { show: horizontal } },
      yaxis: { lines: { show: !horizontal } },
    },
    fill: { opacity: 1 },
    stroke: { width: 0 },
    legend: { show: false },
    tooltip: { y: { formatter: (v) => `${v}` } },
  };
}

export function groupedBarChart(
  categories: string[],
  series: { name: string; data: number[]; color?: string }[],
): BarChartOptions {
  return {
    series: series.map((s) => ({ name: s.name, data: s.data })),
    chart: {
      type: 'bar',
      height: Math.max(280, (categories.length || 1) * 48 + 80),
      toolbar: { show: false },
      stacked: false,
      ...baseFont,
    },
    plotOptions: {
      bar: {
        horizontal: true,
        borderRadius: 5,
        barHeight: '62%',
      },
    },
    colors: series.map((s, i) => s.color ?? REPORT_COLORS[i % REPORT_COLORS.length]),
    dataLabels: { enabled: false },
    xaxis: {
      categories: categories.length ? categories : ['—'],
      labels: { style: { colors: '#a5a3ae', fontSize: '12px' } },
    },
    yaxis: {
      labels: { style: { colors: '#5d596c', fontSize: '12px', fontWeight: 600 } },
    },
    grid: {
      borderColor: 'rgba(47, 43, 61, 0.08)',
      strokeDashArray: 4,
    },
    fill: { opacity: 1 },
    stroke: { width: 0 },
    legend: {
      position: 'top',
      horizontalAlign: 'left',
      fontSize: '13px',
    },
    tooltip: { shared: true, intersect: false },
  };
}

export function radialPercent(value: number, label: string, color = '#8c57ff'): RadialChartOptions {
  const clamped = Math.max(0, Math.min(100, Math.round(value)));
  return {
    series: [clamped],
    chart: { type: 'radialBar', height: 280, ...baseFont },
    labels: [label],
    colors: [color],
    plotOptions: {
      radialBar: {
        hollow: { size: '62%' },
        track: { background: 'rgba(140, 87, 255, 0.12)' },
        dataLabels: {
          name: { fontSize: '13px', color: '#a5a3ae', offsetY: 20 },
          value: {
            fontSize: '28px',
            fontWeight: 700,
            color: '#5d596c',
            offsetY: -12,
            formatter: (v) => `${v}%`,
          },
        },
      },
    },
    fill: { type: 'solid' },
    stroke: { lineCap: 'round' },
  };
}

export function radialScore(value: number, max = 5, label = 'Average'): RadialChartOptions {
  const pct = max > 0 ? Math.round((value / max) * 100) : 0;
  return {
    series: [pct],
    chart: { type: 'radialBar', height: 280, ...baseFont },
    labels: [label],
    colors: ['#56ca00'],
    plotOptions: {
      radialBar: {
        hollow: { size: '62%' },
        track: { background: 'rgba(86, 202, 0, 0.12)' },
        dataLabels: {
          name: { fontSize: '13px', color: '#a5a3ae', offsetY: 20 },
          value: {
            fontSize: '28px',
            fontWeight: 700,
            color: '#5d596c',
            offsetY: -12,
            formatter: () => value.toFixed(1),
          },
        },
      },
    },
    fill: { type: 'solid' },
    stroke: { lineCap: 'round' },
  };
}
