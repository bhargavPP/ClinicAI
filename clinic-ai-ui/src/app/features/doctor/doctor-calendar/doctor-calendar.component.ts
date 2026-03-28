import { Component, OnInit } from '@angular/core';
import { CalendarOptions } from '@fullcalendar/core';
import { FullCalendarModule } from '@fullcalendar/angular';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import { AppointmentService } from '../../../core/services/appointment.service';
import { CommonModule } from '@angular/common';
import { AppointmentModel } from '../../../shared/appointment-model/appointment-model';
@Component({
  selector: 'app-doctor-calendar',
  standalone: true,
  imports: [FullCalendarModule, CommonModule,AppointmentModel],
  templateUrl: './doctor-calendar.component.html',
  styleUrl: './doctor-calendar.component.css',
})
export class DoctorCalendarComponent implements OnInit {

  // 👉 selected event for modal (instead of alert)
  selectedEvent: any = null;

  calendarOptions: CalendarOptions = {
    plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
    initialView: 'timeGridWeek',

    slotMinTime: '08:00:00',
    slotMaxTime: '20:00:00',
    slotDuration: '00:20:00',
    allDaySlot: false,

    nowIndicator: true,
    height: 'auto',
    scrollTime: '09:00:00',

    editable: true, // ✅ enable drag/drop

    headerToolbar: {
      left: 'prev,next today',
      center: 'title',
      right: 'timeGridWeek,dayGridMonth'
    },

    // ✅ replace alert with modal trigger
    eventClick: (info) => {
      this.selectedEvent = info.event;
    },

    // ✅ drag & drop reschedule
    eventDrop: (info) => {
      const event = info.event;

      const updated = {
        id: event.id,
        start: event.start,
        end: event.end
      };

    },

    // ✅ better UI rendering
    eventContent: (arg: any) => {
      const status = arg.event.extendedProps['status'];

      const statusBg =
        status === 'Scheduled' ? '#d4edda' :
          status === 'Cancelled' ? '#f8d7da' :
            '#e2e3e5';

      return {
        html: `
      <div style="
        padding:4px;
        border-radius:6px;
        font-size:12px;
        line-height:1.2;
      ">
        <div style="font-weight:600;">${arg.event.title}</div>
        <div style="opacity:0.7;">${arg.timeText}</div>
        <div style="
          margin-top:3px;
          padding:2px 6px;
          border-radius:4px;
          background:${statusBg};
          display:inline-block;
          font-size:11px;
        ">
          ${status}
        </div>
      </div>
    `
      };
    },

    // ✅ tooltip on hover
    eventDidMount: (info) => {
      info.el.title = `
Patient: ${info.event.title}
Status: ${info.event.extendedProps['status']}
Notes: ${info.event.extendedProps['notes']}
      `;
    }
  };

  constructor(private appointmentService: AppointmentService) { }

  ngOnInit(): void {
    this.loadAppointments();
  }

  // ✅ separated for reuse
  loadAppointments() {
    this.appointmentService.getMyAppointments().subscribe((res: any[]) => {

      const events = res.map(a => {
        const date = a.date.split('T')[0];

        return {
          id: a.id,
          title: a.patientName,
          start: `${date}T${a.startTime}`,
          end: `${date}T${a.endTime}`,

          backgroundColor: 'transparent',
          borderColor:
            a.status === 'Scheduled' ? '#28a745' :
              a.status === 'Cancelled' ? '#dc3545' :
                '#6c757d',

          textColor: '#000',

          extendedProps: {
            notes: a.notes,
            status: a.status
          }
        };
      });

      this.calendarOptions = {
        ...this.calendarOptions,
        events
      };
    });
  }
}
