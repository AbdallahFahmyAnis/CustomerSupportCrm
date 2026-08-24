import {
  Column,
  Entity,
  PrimaryColumn,
} from 'typeorm';

/** SDD CRM-012 / CRM-037 — portal request row (Postgres via TypeORM). */
@Entity({ name: 'portal_requests' })
export class PortalRequestEntity {
  @PrimaryColumn('varchar', { length: 64 })
  id!: string;

  @Column('varchar', { length: 64 })
  ticketId!: string;

  @Column('varchar', { length: 64 })
  ticketNumber!: string;

  @Column('varchar', { length: 64 })
  customerId!: string;

  @Column('varchar', { length: 320 })
  email!: string;

  @Column('varchar', { length: 200 })
  name!: string;

  @Column('varchar', { length: 500 })
  subject!: string;

  @Column('varchar', { length: 64 })
  status!: string;

  @Column('varchar', { length: 40 })
  createdAt!: string;
}

/** SDD CRM-012 / CRM-037 — channel message row. */
@Entity({ name: 'channel_messages' })
export class ChannelMessageEntity {
  @PrimaryColumn('varchar', { length: 64 })
  id!: string;

  @Column('varchar', { length: 64 })
  ticketId!: string;

  @Column('varchar', { length: 32 })
  channel!: string;

  @Column('varchar', { length: 32 })
  direction!: string;

  @Column('text')
  body!: string;

  @Column('varchar', { length: 320, nullable: true })
  fromEmail!: string | null;

  @Column('varchar', { length: 40 })
  createdAt!: string;
}
